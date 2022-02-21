using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour{

    [Tooltip("All children UI elements")]
    public GameObject[] uiElements;

    public void closeAllUI(){
        foreach (GameObject obj in uiElements){
            obj.SetActive(false);
        }
    }

    public void openUI(GameObject element){
        closeAllUI();
        element.SetActive(true);
    }

}
