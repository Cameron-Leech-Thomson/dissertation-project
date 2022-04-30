using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandGrab : MonoBehaviour
{

    /* 
    *   Makes sure that both controllers can't be holding the same object at the same time.
    */

    public void StopGrip(XRDirectInteractor controller){
        StartCoroutine(stopGripRoutine(controller));
    }

    private IEnumerator stopGripRoutine(XRDirectInteractor controller){
        controller.allowHover = false;
        controller.allowSelect = false;
        yield return new WaitForSeconds(0.25f);
        controller.allowHover = true;
        controller.allowSelect = true;
    }

}
