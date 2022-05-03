using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class WallButtonTrigger : MonoBehaviour, ButtonTrigger
{

    [Tooltip("The game object of the interactable part of the button")]
    public GameObject buttonSwitch;

    [Tooltip("The physics controller object in the scene")]
    public PhysicsController physics;

    [Tooltip("Force required to activate the button")]
    public float forceRequired = 12f;

    [Tooltip("Whether or not the button can be used multiple times")]
    public bool oneTimeUse = false;
    bool activated = false;

    [Tooltip("The game object(s) that will be activated once this button has been pressed")]
    public GameObject[] activates;

    [Tooltip("Activates whenever something touches the button, regardless if it will successfuly activate or not")]
    public GameObject[] alwaysActivate = null;

    Canvas forceCanvas;

    MeshCollider buttonCollider;

    Vector3 buttonUp;
    Vector3 buttonDown;

    // Start is called before the first frame update
    void Start()
    {
        forceCanvas = GetComponentInChildren<Canvas>();
        forceCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText("Force Required:\n" + forceRequired);

        buttonCollider = buttonSwitch.GetComponent<MeshCollider>();
        ColliderBridge cb = buttonCollider.gameObject.AddComponent<ColliderBridge>();
        cb.Initalize(this);

        Vector3 offsetVector = new Vector3(0.05f, 0.0f, 0.05f) * transform.lossyScale.y;

        Transform buttonTransform = buttonSwitch.transform;
        buttonUp = buttonTransform.position;
        buttonDown = buttonUp - multiplyVectors(offsetVector, buttonSwitch.transform.up);
    }

    void Update() {
        forceCanvas.transform.LookAt(physics.playerRig.GetComponentInChildren<Camera>().transform);
    }

	private Vector3 multiplyVectors(Vector3 v1, Vector3 v2){
		Vector3 product = new Vector3(0, 0, 0);
		for (int i = 0; i < 3; i++)
		{
			product[i] = v1[i] * v2[i];
		}
		return product;
	}

    public void OnCollisionEnter(Collision collisionInfo) {
        if (!activated){
            // Get object that is interacting with the button:
            GameObject dynObject = collisionInfo.gameObject;

            // Check if the object colliding with the button is a valid interactable:
            if (dynObject.GetComponent<XRGrabInteractable>() != null){
                // Get the rigidbody belonging to the dynamic object:
                Rigidbody rb = dynObject.GetComponent<Rigidbody>();

                Vector3 acc = dynObject.GetComponent<GetAcceleration>().getAcceleration();
                Vector3 vel = dynObject.GetComponent<GetAcceleration>().getLastVelocity();
                
                float force1 = Mathf.Abs(calculateForce(acc, rb.mass));
                // Calculate force with velocity in case LaunchObject is used:
                // Dampen it with a constant because it's a bit unfair if not.
                float force2 = Mathf.Abs(calculateForce(vel, rb.mass)) * 0.5f;

                if (force1 > forceRequired || force2 > forceRequired){
                    activated = true;
                    activateButton();
                }                
            }
        }
    }

    float calculateForce(Vector3 acc, float mass){
        // Get where the button is facing to figure out which component of the velocity vector to check:
        Vector3 facing = buttonSwitch.transform.up;

        float acceleration;
        if (facing.x != 0.0f){
            acceleration = Mathf.Abs(acc.x);
        } else {
            acceleration = Mathf.Abs(acc.z);
        }

        // Calculate the force applied (includes relativistic mass from SoL calculations in PhysicsController):
        return mass * acceleration;
    }

    public void activateButton(){
        // Move the button down:
        StartCoroutine(LerpPosition.LerpPos(buttonSwitch.transform, buttonDown, 3));

        /* TODO: Call the activate command of whatever it is connected to.
            Need to check if it needs to be activated or deactivated.
        */
        foreach (GameObject obj in activates){
			// Get the activator component of the object:
            Activatable[] activators = obj.GetComponents<Activatable>();
            foreach (Activatable activator in activators){
                if (activator != null){
				    activator.activate();
			    }
            }
		}

        forceCanvas.gameObject.SetActive(false);
    }

    public void OnCollisionExit(Collision collisionInfo) {
		// Set button back to original position if the button is to be moved again:
        if (!oneTimeUse){
            StartCoroutine(LerpPosition.LerpPos(buttonSwitch.transform, buttonUp, 3));
			activated = false;
        }      
    }

}
