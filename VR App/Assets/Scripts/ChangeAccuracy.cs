using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAccuracy : MonoBehaviour
{
    
    bool isSelected;
    Button button;
    
    [Tooltip("The slider who's accuracy should be changed")]
    public Slider slider;

    void Start() {
        button = gameObject.GetComponent<Button>();
        isSelected = false;
    }

    public void buttonSelected(){
        isSelected = !isSelected;
        if (isSelected){
            slider.maxValue = 50;
            slider.value = 50;
        } else{
            slider.maxValue = 300000000;
            slider.value = 300000000;
        }
    }

}
