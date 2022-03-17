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
            } if (dopplerShift) {
                return "";
            } else{
                return "";
            }
        }
    }

    public Units units = new Units();

    private string getSliderValue(){
        if (units.speedOfLight){
            return slider.value.ToString(new string('#', 339));
        } else{
            return Math.Round(slider.value, 2).ToString();
        }
        
    }

    public void setText(string text){
        TextMeshProUGUI tmp = transform.GetComponent<TextMeshProUGUI>();
        tmp.SetText(text + " - " + getSliderValue() + units.getUnits());
    }

}
