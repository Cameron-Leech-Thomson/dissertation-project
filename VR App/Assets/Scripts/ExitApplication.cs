using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitApplication : MonoBehaviour
{
    
    public void exitApplication(){
        #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
        #else
         Application.Quit();
        #endif
    }

}
