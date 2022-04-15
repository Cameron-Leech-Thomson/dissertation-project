using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GetVelocity : MonoBehaviour
{

    Vector3 velocity = Vector3.zero;

    void OnTriggerEnter(Collider collisionInfo)
    {
        // Get object that is interacting with the trigger:
        GameObject dynObject = collisionInfo.gameObject;
        // Check if the object is a valid interactable:
        if (dynObject.GetComponent<XRGrabInteractable>() != null || dynObject.Equals(transform.parent.gameObject)){
            Debug.Log("Triggered by " + dynObject.name);
            Rigidbody rb = dynObject.GetComponent<Rigidbody>();
            velocity = rb.velocity;
            Debug.Log("Vel: " + velocity.ToString());
        }
    }

    public Vector3 getVelocity(){
        return velocity;
    }
    
}
