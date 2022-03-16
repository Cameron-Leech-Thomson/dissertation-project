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
    [Tooltip("Each slider belonging to the physics UI element.")]
    public PhysicsMenu physicsSliders = new PhysicsMenu();
    List<GameObject> physObjects = new List<GameObject>();
    [Tooltip("Minimum tolerance to consider an object to be moving or not:")]
    public float velocityThreshold = 0.05f;
    [Tooltip("Multiplier to make the value of gravity feel more floaty")]
    public float gravityMultiplier = 0.75f;
    Vector3 minVelocity;
    float gravity = 9.81f;
    int speedOfLight = 300000000;
    int doppler = 300000000;

    // Start is called before the first frame update
    void Start()
    {
        minVelocity = new Vector3(velocityThreshold, velocityThreshold, velocityThreshold);
        foreach(Transform child in physicsObjectsContainer.transform){
            physObjects.Add(child.gameObject);
        }
    }

    void FixedUpdate() {
        // ------------------------  GRAVITY  ------------------------
        gravity = physicsSliders.gravitySlider.value;

        foreach(GameObject physObj in physObjects){
            // Get the rigidbody component of the physics objects:
            Rigidbody rb = physObj.GetComponent<Rigidbody>();
            // Set gravity to false so we can add our custom force of gravity:
            rb.useGravity = false;
            // Add custom gravity force:
            rb.AddForce(new Vector3(0, -1.0f, 0) * rb.mass * gravity * gravityMultiplier);
        }

        // ------------------------  SPEED OF LIGHT  ------------------------
        
        speedOfLight = (int)physicsSliders.speedOfLightSlider.value;

        foreach(GameObject physObj in physObjects){
            // Only affect the object if it isn't being held, or isn't moving:
            if (!physObj.GetComponent<XRGrabInteractable>().isSelected || 
                physObj.GetComponent<Rigidbody>().velocity.magnitude > minVelocity.magnitude){
                // Get the rigidbody components of the physics objects:
                Rigidbody rb = physObj.GetComponent<Rigidbody>();
                // Get the rest values of the object from the ValuesAtRest script:
                ValuesAtRest restVals = physObj.GetComponent<ValuesAtRest>();
                if (!rb.IsSleeping()){
                    // Set mass to relativistic mass:
                    rb.mass = getRelativeMass(rb, restVals);
                    // Set size to relativistic size:
                    setScale(physObj, getRelativeSize(rb, restVals));
                }
            }
        } 

        // ------------------------  DOPPLER SHIFT  ------------------------
        doppler = (int)physicsSliders.dopplerShiftSlider.value;

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

        Vector3 relativeSize = new Vector3(lorentzCalculation(velocity.x, restLength.x),
                                           lorentzCalculation(velocity.y, restLength.y),
                                           lorentzCalculation(velocity.z, restLength.z));
        
        return relativeSize;
    }

    private float lorentzCalculation(float vel, float len){
        return len * (float)Math.Sqrt(Math.Abs(1 - (square(vel) / square(speedOfLight))));
    }

    private float square(float val){
        return val * val;
    }

    public void resetValues(){
        physicsSliders.gravitySlider.value = 9.81f;
        physicsSliders.speedOfLightSlider.value = 300000000;
        physicsSliders.dopplerShiftSlider.value = 300000000;
    }

    public void resetOthers(Slider target){
        if (target.Equals(physicsSliders.gravitySlider)){
            physicsSliders.speedOfLightSlider.value = 300000000;
            physicsSliders.dopplerShiftSlider.value = 300000000;
        } if (target.Equals(physicsSliders.speedOfLightSlider)){
            physicsSliders.gravitySlider.value = 9.81f;
            physicsSliders.dopplerShiftSlider.value = 300000000;
        } if (target.Equals(physicsSliders.dopplerShiftSlider)) {
            physicsSliders.gravitySlider.value = 9.81f;
            physicsSliders.speedOfLightSlider.value = 300000000;
        }
    }

    [Serializable]
    public class PhysicsMenu{
        public Slider gravitySlider;

        public Slider speedOfLightSlider;

        public Slider dopplerShiftSlider;

    }

}
