using System.Collections;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class LaunchObject : MonoBehaviour
{
    
    XRDirectInteractor interactor;
    public XRInteractionManager interactionManager;

    public Slider powerBar;

    public bool isSelected = false;
    public bool isAiming = false;

    public float maximumPower = 20f;

    float currentPower = 0;

    void Start() {
        interactor = GetComponent<XRDirectInteractor>();
        powerBar.value = 0;
        powerBar.maxValue = maximumPower;
        powerBar.gameObject.SetActive(false);
    }

    void LateUpdate() {
        if (isSelected && isAiming){
            powerBar.gameObject.SetActive(true);
            if (currentPower <= maximumPower){
                currentPower += 0.5f;
                powerBar.value = currentPower;
            }
        } else{
            currentPower = 0;
            powerBar.value = currentPower;
            powerBar.gameObject.SetActive(false);
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
            powerBar.gameObject.SetActive(false);
            // Get the interactable that is currently selected:
            XRBaseInteractable heldItem = interactor.selectTarget;
            interactor.allowSelect = false;
            // Deselect the interactable before launching:
            try{
                // Try to deselect the object:
                interactionManager.SelectExit(interactor, heldItem);
            } catch(NullReferenceException){
                // In the case that the user has already let go and a NullReferenceException is thrown,
                // return the users ability to select:
                StartCoroutine(resetSelect());
                return;
            }
            
            Rigidbody rb = heldItem.gameObject.GetComponent<Rigidbody>();

            float offset = Mathf.Abs(Mathf.Log(rb.mass));
            if (offset == 0){
                offset = 1.5f;
            }

            Vector3 fwdVector = gameObject.transform.forward;
            rb.AddForce(fwdVector * (currentPower / offset), ForceMode.VelocityChange);

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
