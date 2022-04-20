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

    [Tooltip("The player's XR Rig")]
    public GameObject playerRig;

    [Tooltip("Each slider belonging to the physics UI element.")]
    public PhysicsMenu physicsSliders = new PhysicsMenu();

    List<GameObject> physObjects = new List<GameObject>();

    [Tooltip("Minimum tolerance to consider an object to be moving or not:")]
    public float velocityThreshold = 0.1f;
    [Tooltip("Multiplier to make the value of gravity feel more floaty")]
    public float gravityMultiplier = 0.75f;
    float gravity = 9.81f;
    int speedOfLight = 300000000;
    int doppler = 0;

    float dist;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in physicsObjectsContainer.transform){
            physObjects.Add(child.gameObject);
            child.gameObject.AddComponent<ValuesAtRest>();
        }
    }

    void FixedUpdate() {
        gravity = physicsSliders.gravitySlider.value;

        speedOfLight = (int)physicsSliders.speedOfLightSlider.value;

        doppler = (int)physicsSliders.dopplerShiftSlider.value;

        foreach(GameObject physObj in physObjects){

            // ------------------------  GRAVITY  ------------------------

            // Get the rigidbody component of the physics objects:
            Rigidbody rb = physObj.GetComponent<Rigidbody>();
            // Set gravity to false so we can add our custom force of gravity:
            rb.useGravity = false;
            // Add custom gravity force:
            rb.AddForce(new Vector3(0, -1.0f, 0) * rb.mass * gravity * gravityMultiplier);

            // ------------------------  SPEED OF LIGHT  ------------------------

            // Only affect the object if it's moving:
            if (!physObj.GetComponent<XRGrabInteractable>().isSelected && 
                physObj.GetComponent<Rigidbody>().velocity.magnitude > velocityThreshold){
                // Debug.Log("GAMEOBJECT: " + physObj.name + " is moving at " + physObj.GetComponent<Rigidbody>().velocity.magnitude.ToString("F5"));
                // Get the rigidbody components of the physics objects:
                // Get the rest values of the object from the ValuesAtRest script:
                ValuesAtRest restVals = physObj.GetComponent<ValuesAtRest>();
                if (!rb.IsSleeping()){
                    // Set mass to relativistic mass:
                    rb.mass = getRelativeMass(rb, restVals);
                    // Set size to relativistic size:
                    setScale(physObj, getRelativeSize(rb, restVals));
                }

            // ------------------------  DOPPLER SHIFT  ------------------------

                // Get bool for direction of movement in relation to player (true towards, false away):
                bool direction = CheckDirection(physObj);
                float vel = rb.velocity.magnitude;

                // Scale the velocity to a hue value:
                float hue = velocityToHue(direction, vel);

                // Add the hue to the materials:
                try{
                    ValuesAtRest valuesAtRest = physObj.GetComponent<ValuesAtRest>();
                    valuesAtRest.setColour(Color.HSVToRGB(hue, 40f, 40f));
                } catch(NullReferenceException e){
                    Debug.Log(e.Message + " - No component found when searching for <ValuesAtRest> in " + physObj.name);
                }
            } else {
                // Reset the object's colour:
                ValuesAtRest valuesAtRest = physObj.GetComponent<ValuesAtRest>();
                if (valuesAtRest.isColourChanged()){
                    valuesAtRest.resetColours();
                }
            } if (physObj.GetComponent<XRGrabInteractable>().isSelected){
                // If the object is selected...
                ValuesAtRest valuesAtRest = physObj.GetComponent<ValuesAtRest>();
                // Reset the scale:
                Vector3 restLength = valuesAtRest.getRestLength();
                if (Vector3.Distance(physObj.transform.lossyScale, restLength) > restLength.magnitude/4){
                    setScale(physObj, restLength);
                }
                // Reset the colour:
                if (valuesAtRest.isColourChanged()){
                    valuesAtRest.resetColours();
                }
            } 
        }
    }

    public float velocityToHue(bool direction, float vel){
        vel = Mathf.Abs(vel * (doppler / 10));
        // Hue values between red & blue are 240 - 360, with a midpoint of 300:
        if (vel > 60){
            vel = 60;
        } if (direction){
            vel = -vel;
        }
        
        // In Unity, Hue is measured between 0 - 1, so map the value to that scale:
        return (300 + vel) / 360;
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

    private void setScale(GameObject obj, Vector3 scale){
        obj.transform.parent = null;
        obj.transform.localScale = scale;
        obj.transform.parent = physicsObjectsContainer.transform;
    }

    private float getRelativeMass(Rigidbody rb, ValuesAtRest restVals) {
        // Get velocity & convert to a single value:
        float velocity = rb.velocity.magnitude;
        // Get the rest mass of the object:
        float restMass = restVals.getRestMass();

        // Calculate relativistic mass:
        float relativeMass = restMass / (float)(Math.Sqrt(Math.Abs(1 - (square(velocity) / square(speedOfLight)))));

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

    public float[] getValues(){
        float[] vals = {gravity, speedOfLight, doppler};
        return vals;
    }

    public void resetValues(){
        physicsSliders.gravitySlider.value = 9.81f;
        physicsSliders.speedOfLightSlider.value = 300000000;
        physicsSliders.dopplerShiftSlider.value = 0;
    }

    public void resetOthers(Slider target){
        // Reset maximum value if it has been reduced:
        if(!target.Equals(physicsSliders.speedOfLightSlider)){
            physicsSliders.speedOfLightSlider.maxValue = 300000000f;
        }

        float[] defaults = {9.81f, 300000000f, 0};
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
        public Slider gravitySlider;

        public Slider speedOfLightSlider;

        public Slider dopplerShiftSlider;

        public Slider[] GetSliders(){
            return new Slider[] {gravitySlider, speedOfLightSlider, dopplerShiftSlider};
        }

    }

}
