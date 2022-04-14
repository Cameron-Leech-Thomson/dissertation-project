using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonTrigger : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        buttonCollider = buttonSwitch.GetComponent<MeshCollider>();
        ColliderBridge cb = buttonCollider.gameObject.AddComponent<ColliderBridge>();
        cb.Initalize(this);

        buttonUp = buttonSwitch.transform.position;
        buttonDown = buttonUp - new Vector3(0, 0.045f, 0);
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {
        if (!activated){
            // Get object that is interacting with the button:
            GameObject dynObject = collisionInfo.gameObject;
            // Check if the object colliding with the button is a valid interactable:
            if (dynObject.GetComponent<XRGrabInteractable>() != null){
                // Get the rigidbody belonging to the dynamic object:
                Rigidbody rb = dynObject.GetComponent<Rigidbody>();
                // Calculate the downward force it is exerting:
                float gravity = physics.getValues()[0];
                float downForce = rb.mass * gravity * physics.gravityMultiplier;

                if (downForce > forceRequired){
					activated = true;
                    activateButton();
                }
            }
        }
    }

    void activateButton(){
        // Move the button down:
        StartCoroutine(LerpPosition(buttonDown, 3));

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

    public void OnCollisionExit(Collision collisionInfo)
    {
        // Set button back to original position if the button is to be moved again:
        if (!oneTimeUse){
            StartCoroutine(LerpPosition(buttonUp, 3));
			activated = false;
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = buttonSwitch.transform.position;
        while (time < duration)
        {
            buttonSwitch.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        buttonSwitch.transform.position = targetPosition;
    }

}
