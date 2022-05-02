using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class PhysicsController : MonoBehaviour
{

    [Tooltip("The parent object containing all dynamic physics objects.")]
    public GameObject physicsObjectsContainer;

    [Tooltip("Prefab for the show mass UI")]
    public GameObject showMassPrefab;

    [Tooltip("The player's XR Rig")]
    public GameObject playerRig;

    [Tooltip("The minimum height an object can be at before it is returned to it's starting position")]
    public float minimuimHeight = -10f;

    public float velocityThreshold = 0.2f;

    [Tooltip("Each slider belonging to the physics UI element.")]
    public PhysicsMenu physicsSliders = new PhysicsMenu();

    public GameObject[] controllers;

    List<GameObject> physObjects = new List<GameObject>();

    [Tooltip("Minimum tolerance to consider an object to be moving or not:")]
    Vector3 minVelocity;
    float gravity = PhysicsMenu.Gravity;
    int speedOfLight = PhysicsMenu.SpeedOfLight;
    int doppler = PhysicsMenu.Doppler;

    float dist;

    private bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        // Add functionality to all dynamic objects at runtime:
        foreach(Transform child in physicsObjectsContainer.transform){
            if (child.gameObject.tag.Equals("PhysContainer")){
                foreach(Transform subChild in child.transform){
                    GameObject childObj = subChild.gameObject;
                    if (childObj.GetComponent<XRBaseInteractable>() != null){
                        physObjects.Add(childObj);
                        childObj.AddComponent<ValuesAtRest>(); 
                        childObj.AddComponent<GetAcceleration>();
                    
                        GameObject massUI = Instantiate(showMassPrefab) as GameObject;
                        massUI.transform.SetParent(childObj.transform,false);
                        massUI.GetComponent<ShowMass>().init(playerRig);

                        // Set gravity to false so we can add our custom force of gravity:
                        childObj.GetComponent<Rigidbody>().useGravity = false;

                        if(childObj.GetComponent<ResetPositionActivatable>() == null){
                            ResetPositionActivatable resetPos = childObj.AddComponent<ResetPositionActivatable>();
                            resetPos.activateAnyTrigger = false;
                        }
                    }
                }
            }
        }
        minVelocity = new Vector3(velocityThreshold, velocityThreshold, velocityThreshold);
        isStarted = true;
    }

    void FixedUpdate()
    {
        if (isStarted){

            gravity = physicsSliders.gravitySlider.value;

            speedOfLight = (int)physicsSliders.speedOfLightSlider.value;

            doppler = (int)physicsSliders.dopplerShiftSlider.value;

            foreach (GameObject physObj in physObjects){
                ValuesAtRest restVals = physObj.GetComponent<ValuesAtRest>();
                // Reset object if it has fallen past the minimum height:
                if (physObj.transform.position.y < minimuimHeight || Vector3.Distance(physObj.transform.position, restVals.restPos) > 50f){
                    physObj.GetComponent<ResetPositionActivatable>().activate();
                }

                // Get the rigidbody component of the physics objects:
                Rigidbody rb = physObj.GetComponent<Rigidbody>();

                if (!physObj.GetComponent<XRGrabInteractable>().isSelected){

                    // ------------------------  GRAVITY  ------------------------

                    // Add custom gravity force...
                    // Add gravity, gravity changes relatively, so it would always appear to change the object at the same rate,
                    // Hence the rest mass being used rather than the relative mass:
                    rb.AddForce(Vector3.down * restVals.getRestMass() * gravity, ForceMode.Acceleration);

                    // if (Math.Abs(rb.velocity.y) > velocityThreshold && rb.velocity.y < 0){
                    //     Debug.Log("Downward Velocity of: " + rb.velocity.y);
                    // }

                    // ------------------------  SPEED OF LIGHT  ------------------------

                    // Only affect the object if it's moving:
                    if (Math.Abs(physObj.GetComponent<Rigidbody>().velocity.magnitude) > minVelocity.magnitude){
                        // Debug.Log("GAMEOBJECT: " + physObj.name + " is moving at " + physObj.GetComponent<Rigidbody>().velocity.magnitude.ToString("F5"));
                        // Get the rigidbody components of the physics objects:
                        // Get the rest values of the object from the ValuesAtRest script:
                        if (!rb.IsSleeping()){
                            // Set mass to relativistic mass:
                            Vector3 vel = rb.velocity;
                            rb.mass = getRelativeMass(rb, restVals);
                            rb.velocity = vel;
                            // Set size to relativistic size:
                            setScale(physObj, getRelativeSize(rb, restVals), restVals.getRestLength());
                        }

                        // ------------------------  DOPPLER SHIFT  ------------------------

                        if (doppler > 0){
                            // Get bool for direction of movement in relation to player (true towards, false away):
                            bool direction = CheckDirection(physObj);
                            float vel = rb.velocity.magnitude;

                            // Scale the velocity to a hue value:
                            float hue = velocityToHue(direction, vel);
                            // Debug.Log("Velocity of " + vel + ", Hue of " + hue);

                            // Add the hue to the materials:
                            try{
                                ValuesAtRest valuesAtRest = physObj.GetComponent<ValuesAtRest>();
                                valuesAtRest.setColour(Color.HSVToRGB(hue, 0.75f, 0.75f));
                            }catch (NullReferenceException e)
                            {
                                Debug.Log(e.Message + " - No component found when searching for <ValuesAtRest> in " + physObj.name);
                            }
                        }
                    }else{
                        // Reset the object's colour:
                        Vector3 restLength = restVals.getRestLength();
                        if (restLength != Vector3.zero){
                            // If the current scale isn't within 0.1 of the original:
                            if (Vector3.Distance(physObj.transform.lossyScale, restLength) > 0.1f){
                                // Reset:
                                setScale(physObj, restLength, restLength);
                            }
                        }if (restVals.isColourChanged())
                        {
                            restVals.resetColours();
                        }
                    }
                } else{
                    // If the object is selected...
                    ValuesAtRest valuesAtRest = physObj.GetComponent<ValuesAtRest>();
                    // Reset the scale:
                    Vector3 restLength = valuesAtRest.getRestLength();
                    setScale(physObj, restLength, valuesAtRest.getRestLength());
                    // Reset the colour:
                    if (valuesAtRest.isColourChanged())
                    {
                        valuesAtRest.resetColours();
                    }
                    // Reset velocity:
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    public float velocityToHue(bool direction, float vel){
        vel = Mathf.Clamp(Mathf.Abs(vel * (doppler / 10)), 0, 60);
        // Hue values between red & blue are 240 - 360, with a midpoint of 300:
        if (direction){
            vel = -vel;
        }
        
        // In Unity.Color, Hue is measured between 0 - 1, so map the value to that scale:
        return (300f + vel) / 360f;
    }

    // Return true if the object is travelling towards the player, false if away:
    public bool CheckDirection(GameObject obj) {
        float distTemp = Vector3.Distance(playerRig.transform.position, obj.transform.position);
        if (distTemp < dist) {
            dist = distTemp;
            return true;
        } else if (distTemp > dist) {
            dist = distTemp;
            return false;
        }
        return false;
    }

    private void setScale(GameObject obj, Vector3 scale, Vector3 restLength){
        Vector3 velocity = obj.GetComponent<Rigidbody>().velocity;
        Transform parent = obj.transform.parent;
        // If the new scale is smaller than 1/3 the original size, cap it at 1/3:
        if (Vector3.Distance(scale, restLength) > Vector3.Distance(restLength, restLength / 2.5f)){
            scale = restLength / 2.5f;
        }
        obj.transform.parent = null;
        obj.transform.localScale = scale;
        obj.transform.parent = parent;
        obj.GetComponent<Rigidbody>().velocity = velocity;
    }

    private float getRelativeMass(Rigidbody rb, ValuesAtRest restVals) {
        // Get velocity & convert to a single value:
        float velocity = rb.velocity.magnitude;
        // Get the rest mass of the object:
        float restMass = restVals.getRestMass();

        // Calculate relativistic mass:
        float ratio = square(velocity) / square(speedOfLight);
        if (ratio >= 1){
            ratio = 0.9999f;
        }
        float relativeMass = restMass / (float)(Math.Sqrt(1 - ratio));

        return relativeMass;
    }

    private Vector3 getRelativeSize(Rigidbody rb, ValuesAtRest restVals){
        // Get velocity:
        Vector3 velocity = rb.velocity;
        // Get the rest length:
        Vector3 restLength = restVals.getRestLength();

        Vector3 relativeSize = new Vector3(lorentzCalculation(Math.Abs(velocity.x), restLength.x),
                                           lorentzCalculation(Math.Abs(velocity.y), restLength.y),
                                           lorentzCalculation(Math.Abs(velocity.z), restLength.z));
        
        return relativeSize;
    }

    private float lorentzCalculation(float vel, float len){
        float lorentzFactor;
        // If the lorentz factor would be greater than 1:
        if (vel >= speedOfLight){
            lorentzFactor = 0.9999f;
        } else{
            lorentzFactor = square(vel) / square(speedOfLight);
        }
        return len * (float)Math.Sqrt(Math.Abs(1 - lorentzFactor));
    }

    private float square(float val){
        return val * val;
    }

    internal float[] getValues(){
        float[] vals = {gravity, speedOfLight, doppler};
        return vals;
    }

    public void resetValues(){
        physicsSliders.gravitySlider.value = PhysicsMenu.Gravity;
        physicsSliders.speedOfLightSlider.value = PhysicsMenu.SpeedOfLight;
        physicsSliders.dopplerShiftSlider.value = PhysicsMenu.Doppler;
    }

    public void resetOthers(Slider target){
        // Reset maximum value if it has been reduced:
        if(!target.Equals(physicsSliders.speedOfLightSlider)){
            physicsSliders.speedOfLightSlider.maxValue = PhysicsMenu.SpeedOfLight;
        }

        float[] defaults = {PhysicsMenu.Gravity, PhysicsMenu.SpeedOfLight, PhysicsMenu.Doppler};
        Slider[] sliders = physicsSliders.GetSliders();
        for (int i = 0; i < defaults.Length; i++)
        {
            if (!target.Equals(sliders[i])){
                sliders[i].value = defaults[i];
            }
        }
    }

    [Serializable]
    public class PhysicsMenu{

        // Default value of gravity:
        public const float Gravity = 9.81f;
        // Default value of c:
        public const int SpeedOfLight = 300000000;
        // Default doppler shift value:
        public const int Doppler = 0;

        public Slider gravitySlider;

        public Slider speedOfLightSlider;

        public Slider dopplerShiftSlider;

        public Slider[] GetSliders(){
            return new Slider[] {gravitySlider, speedOfLightSlider, dopplerShiftSlider};
        }
    }

}
