using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChangeText : MonoBehaviour
{
    
    public Slider slider;

    [Serializable]
    public class Units {
        public bool gravity;
        public bool speedOfLight;
        public bool dopplerShift;

        public string getUnits(){
            if (gravity){
                return "m/s²";
            } if (speedOfLight){
                return "m/s";
            } else {
                return "";
            }
        }
    }

    public Units units = new Units();

    private float getSliderValue(){
        return slider.value;
    }

    public void setText(string text){
        TextMeshProUGUI tmp = transform.GetComponent<TextMeshProUGUI>();
        tmp.SetText(text + " - " + getSliderValue() + units.getUnits());
    }

}
