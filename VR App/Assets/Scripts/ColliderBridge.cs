using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    ButtonTrigger _listener;
    public void Initalize(ButtonTrigger l){
        _listener = l;
    }

    void OnCollisionEnter(Collision collisionInfo){
        _listener.OnCollisionEnter(collisionInfo);
    }
}
