using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAcceleration : MonoBehaviour
{

    Rigidbody rb;
    Vector3 lastVelocity = Vector3.zero;
    Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f){
            acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = rb.velocity;
        } else{
            lastVelocity = Vector3.zero;
        }
    }

    internal Vector3 getAcceleration(){
        return acceleration;
    } 
}
