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

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in physicsObjectsContainer.transform){
            Debug.Log("Child Name: " + child.name);
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

        // ------------------------  DOPPLER SHIFT  ------------------------

    }

    [Serializable]
    public class PhysicsMenu{
        public Slider gravitySlider;

        public Slider speedOfLightSlider;

        public Slider dopplerShiftSlider;
    }

}
