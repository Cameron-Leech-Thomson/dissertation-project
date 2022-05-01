using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ElevatorController : MonoBehaviour
{

    public float extensionDistance = 0f;
    public GameObject playerRig;
    public XRInteractionManager interactionManager;
    public LayerMask playerLayer;
    bool beginAscent = false;
    bool beginDescent = false;
    bool ascentComplete = false;

    XRRayInteractor[] rayInteractors;

    float t = 0;
    float initY;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractors = playerRig.GetComponentsInChildren<XRRayInteractor>();
        initY = transform.position.y;
    }

    void OnTriggerEnter(Collider other) {
        GameObject collision = other.gameObject;
        if(LayerMask.GetMask(LayerMask.LayerToName(collision.layer)) == playerLayer){
            // Stop the player from teleporting while the elevator is moving:
            setMovement(false);
            if (!ascentComplete){
                beginAscent = true;    
            } else{
                beginDescent = true;
            }
        }
    }

    void Update() {
        if (beginAscent){
            // Move elevator up:
            float increase = Mathf.Lerp(initY, initY + extensionDistance, t);
            transform.position = new Vector3(transform.position.x, increase, transform.position.z);
            
            // Move player with elevator:
            Vector3 playerPos = playerRig.transform.position;
            playerRig.transform.position = new Vector3(playerPos.x, increase + 0.1f, playerPos.z);

            t += Time.deltaTime / 3;
            
            if (increase == initY + extensionDistance){
                // Elevator stopped so allow movement:
                setMovement(true);
                beginAscent = false;
                ascentComplete = true;
            }
        }
        if (beginDescent){
            ascentComplete = false;
            // Move elevator down:
            float decrease = Mathf.Lerp(initY, initY + extensionDistance, t);
            transform.position = new Vector3(transform.position.x, decrease, transform.position.z);

            // Move player with elevator:
            Vector3 playerPos = playerRig.transform.position;
            playerRig.transform.position = new Vector3(playerPos.x, decrease, playerPos.z);

            t -= Time.deltaTime / 3;

            if (decrease == initY){
                // Elevator stopped so allow movement:
                setMovement(true);
                beginDescent = false;
            }
        }
    }

    void setMovement(bool b){
        foreach(XRRayInteractor interactor in rayInteractors){
                interactor.enabled = b;
            }
    }
}
