using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInTrigger : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other) {
        GameObject obj = other.gameObject;
        
        Activatable activatable = obj.GetComponent<Activatable>();
        if (activatable != null){
            if (activatable is ResetPositionActivatable){
                if(!(activatable as ResetPositionActivatable).activateAnyTrigger){
                    return;
                } 
            }
            if (activatable is LerpActivatable){
                return;
            }
            activatable.activate();
        }
    }

}
