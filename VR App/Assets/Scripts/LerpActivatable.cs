using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpActivatable : MonoBehaviour, Activatable
{

    bool active = false;

    [Tooltip("The resultant movement in the X, Y, & Z directions")]
    public float x,y,z;

    Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        endPos = transform.position - new Vector3(x, y, z);
    }

    public void activate(){
        active = true;
        StartCoroutine(LerpPosition.LerpPos(transform, endPos, 3));
    }

    public void deactivate(){
        active = false;
    }

    public bool isActive(){
        return active;
    }
}
