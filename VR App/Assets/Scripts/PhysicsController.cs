using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PhysicsController : MonoBehaviour
{

    [Tooltip("The parent object containing all dynamic physics objects.")]
    public GameObject physicsObjectsContainer;
    [Tooltip("Each slider belonging to the physics UI element.")]
    public PhysicsMenu physicsSliders = new PhysicsMenu();
    List<GameObject> physObjects = new List<GameObject>();
    float gravity = 9.8f;
    int speedOfLight = 300000000;
    int doppler = 300000000;

    // Start is called before the first frame update
    void Start()
    {
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
            rb.AddForce(new Vector3(0, -1.0f, 0) * rb.mass * gravity);
        }

        // ------------------------  SPEED OF LIGHT  ------------------------
        speedOfLight = (int)physicsSliders.speedOfLightSlider.value;

        foreach(GameObject physObj in physObjects){
            // Get the rigidbody components of the physics objects:
            Rigidbody rb = physObj.GetComponent<Rigidbody>();
            // Get the rest values of the object from the ValuesAtRest script:

            // Set mass to relativistic mass:
            rb.mass = getRelativeMass(rb, restVals);
        }

        // ------------------------  DOPPLER SHIFT  ------------------------
        doppler = (int)physicsSliders.dopplerShiftSlider.value;

    }

    private float getRelativeMass(Rigidbody rb, ValuesAtRest restVals) {
        // Get velocity & convert to a single value:
        float velocity = rb.velocity.magnitude;
        // Get the rest mass of the object:
        float restMass = restVals.getRestMass();

        // Calculate relativistic mass:
        float relativeMass = restMass / (float)(Math.Sqrt(1 - (square(velocity) / square(speedOfLight))));

        return relativeMass;
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
        } else {
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
