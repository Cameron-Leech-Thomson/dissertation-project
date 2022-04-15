using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuesAtRest : MonoBehaviour
{
    
    [Tooltip ("Mass of object at a standstill")]
    float restMass;
    Vector3 restLength;

    void Start() {
        // Get rigidbody component:
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        // Get rest mass from Rigidbody vals:
        restMass = rb.mass;

        // Get relative size of the object from its starting transform:
        restLength = gameObject.transform.lossyScale;
    }

    public Vector3 getRestLength(){
        return restLength;
    }

    public float getRestMass(){
        return restMass;
    }

}
