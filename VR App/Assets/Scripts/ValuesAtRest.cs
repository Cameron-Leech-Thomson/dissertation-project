using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuesAtRest : MonoBehaviour
{
    
    [Tooltip ("Mass of object at a standstill")]
    public float restMass;
    Vector3 restLength;

    void Start() {
        // Get rigidbody component:
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        // Set mass to rest values:
        rb.mass = restMass;

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
