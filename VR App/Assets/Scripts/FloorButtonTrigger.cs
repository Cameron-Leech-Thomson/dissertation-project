using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class FloorButtonTrigger : MonoBehaviour, ButtonTrigger
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

        buttonUp = buttonSwitch.transform.position;
        buttonDown = buttonUp - (new Vector3(0, 0.05f, 0) * transform.lossyScale.y);
    }

    void Update() {
        forceCanvas.transform.LookAt(physics.playerRig.GetComponentInChildren<Camera>().transform);
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
                float downForce = rb.mass * gravity;

                if (downForce > forceRequired){
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

    public void OnCollisionExit(Collision collisionInfo)
    {
        // Set button back to original position if the button is to be moved again:
        if (!oneTimeUse){
            StartCoroutine(LerpPosition.LerpPos(buttonSwitch.transform, buttonUp, 3));
			activated = false;
        }
    }

}
