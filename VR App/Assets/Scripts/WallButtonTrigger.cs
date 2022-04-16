using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

    MeshCollider buttonCollider;

    Vector3 buttonUp;
    Vector3 buttonDown;

    GetVelocity getVel;

    // Start is called before the first frame update
    void Start()
    {
        buttonCollider = buttonSwitch.GetComponent<MeshCollider>();
        ColliderBridge cb = buttonCollider.gameObject.AddComponent<ColliderBridge>();
        cb.Initalize(this);

        Vector3 offsetVector = new Vector3(0.05f, 0.0f, 0.05f) * transform.lossyScale.y;

        Transform buttonTransform = buttonSwitch.transform;
        buttonUp = buttonTransform.position;
        buttonDown = buttonUp - multiplyVectors(offsetVector, buttonSwitch.transform.up);

        getVel = GetComponentInChildren<GetVelocity>();
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
			Debug.Log("Collision with " + dynObject.name);
            // Check if the object colliding with the button is a valid interactable:
            if (dynObject.GetComponent<XRGrabInteractable>() != null){
                // Get the rigidbody belonging to the dynamic object:
                Rigidbody rb = dynObject.GetComponent<Rigidbody>();

                Vector3 vel = getVel.getVelocity();
				Debug.Log("Velocity: " + vel.ToString());
                // Get where the button is facing to figure out which component of the velocity vector to check:
                Vector3 facing = buttonSwitch.transform.up;
                float velDir;
                if (facing.x != 0.0f){
                    velDir = Mathf.Abs(vel.x);
                } else {
                    velDir = Mathf.Abs(vel.z);
                }

                // Calculate the force applied:
                float force = rb.mass * velDir;
				Debug.Log("Force Applied: " + force.ToString());
                if (force > forceRequired){
					activated = true;
                    activateButton();
                }
            }
        }
    }

    public void activateButton(){
        // Move the button down:
        StartCoroutine(LerpPosition.LerpPos(buttonSwitch.transform, buttonDown, 3));

        /* TODO: Call the activate command of whatever it is connected to.
            Need to check if it needs to be activated or deactivated.
        */
        foreach (GameObject obj in activates){
            // Get the activator component of the object:
            Activatable activator = obj.GetComponent<Activatable>();
            if (activator != null){
                activator.activate();
            }
        }
    }

    public void OnCollisionExit(Collision collisionInfo) {
		// Set button back to original position if the button is to be moved again:
        if (!oneTimeUse){
            StartCoroutine(LerpPosition.LerpPos(buttonSwitch.transform, buttonUp, 3));
			activated = false;
        }      
    }

}
