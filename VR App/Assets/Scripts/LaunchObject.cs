using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LaunchObject : MonoBehaviour
{
    
    XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;

    public bool isSelected = false;
    public bool isAiming = false;

    public float maximumPower = 10;

    float currentPower = 0;

    void Start() {
        interactor = GetComponent<XRDirectInteractor>();
    }

    void LateUpdate() {
        if (isSelected && isAiming){
            if (currentPower <= maximumPower){
                currentPower += 1;
            }
        } else{
            currentPower = 0;
        }
    }

    public void itemSelected(bool val){
        isSelected = val;
    }

    public void holdingToFire(bool val){
        isAiming = val;
    }

    public void fire(){
        if (interactor.isSelectActive){
            // Get the interactable that is currently selected:
            XRBaseInteractable heldItem = interactor.selectTarget;
            interactor.allowSelect = false;
            // Deselect the interactable before launching:
            interactionManager.SelectExit(interactor, heldItem);
            
            Rigidbody rb = heldItem.gameObject.GetComponent<Rigidbody>();

            Vector3 fwdVector = gameObject.transform.forward;
            rb.AddForce(fwdVector * (currentPower / Mathf.Log(rb.mass)), ForceMode.VelocityChange);

            StartCoroutine(resetSelect());

            // Reset values:
            currentPower = 0;
            heldItem = null;
            itemSelected(false);
            holdingToFire(false);
        }
    }

    private IEnumerator resetSelect()
    {
        yield return new WaitForSeconds(1.1f);
        
        interactor.allowSelect = true;
    }

}
