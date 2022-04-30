using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowMass : MonoBehaviour
{

    GameObject playerRig;

    bool isInit = false;

    // Update is called once per frame
    void Update()
    {
        if(isInit){
            Vector3 offset;
            if (transform.parent.transform.localScale.y < 1){
                offset = (Vector3.up * transform.parent.transform.lossyScale.y);
                if (transform.parent.transform.localScale.y < 0.1f){
                    offset *= 25f;
                }
            } else{
                offset = (Vector3.up * transform.parent.transform.lossyScale.y / 2.5f);
            }
            Vector3 pos = transform.parent.transform.position + offset;
            transform.position = pos;
            transform.LookAt(playerRig.GetComponentInChildren<Camera>().transform);   
        }
    }

    internal void init(GameObject rig){
        playerRig = rig;

        float restMass = GetComponentInParent<Rigidbody>().mass;
        GetComponentInChildren<TextMeshProUGUI>().SetText("Mass:\n" + restMass + "kg");

        reScale(); 

        isInit = true;
    }

    void reScale(){        
        Vector3 scale = transform.localScale;
        if (transform.parent.transform.localScale.y >= 1){
            scale.x/=2.5f;
            scale.y/=2.5f;
            scale.z/=2.5f;
            transform.localScale = scale;
        } else if (transform.parent.transform.localScale.y < 0.1f) {
            scale.x = 0.5f;
            scale.y = 0.5f;
            scale.z = 0.5f;
            transform.localScale = scale;
        }
    }
}
