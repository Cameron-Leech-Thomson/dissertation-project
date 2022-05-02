using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GetAcceleration : MonoBehaviour
{

    Rigidbody rb;
    Vector3 lastVelocity = Vector3.zero;
    Vector3 acceleration;

    XRBaseInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<XRBaseInteractable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!interactable.isSelected){
            if (rb.velocity.magnitude > 0.1f){
                // Doesn't interact nicely with LaunchObject because it's instant force so there's no acceleration/deceleration
                acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
                lastVelocity = rb.velocity;
            }
        } else{
            lastVelocity = Vector3.zero;
        }
        
    }

    internal Vector3 getLastVelocity(){
        return lastVelocity;
    }

    internal Vector3 getAcceleration(){
        return acceleration;
    } 
}
