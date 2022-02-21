using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnProviderManager : MonoBehaviour
{
    private ActionBasedSnapTurnProvider snapTurn;
    private ActionBasedContinuousTurnProvider contTurn;

    void Start() {
        snapTurn = gameObject.GetComponent<ActionBasedSnapTurnProvider>();
        contTurn = gameObject.GetComponent<ActionBasedContinuousTurnProvider>();
    }

    public void enableSnapTurn(){
        contTurn.enabled = false;
        snapTurn.enabled = true;
    }

    public void enableContTurn(){
        snapTurn.enabled = false;
        contTurn.enabled = true;
    }

}
