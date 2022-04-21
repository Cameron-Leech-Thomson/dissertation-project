using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{

    public float speed;
    Animator animator;
    private float triggerTarget;
    private float gripTarget;

    private float gripCurrent;
    private float triggerCurrent;

    string gripParam = "Grip";
    string triggerParam = "Trigger";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }

    internal void SetGrip(float f){
        gripTarget = f;
    }

    internal void SetTrigger(float f){
        triggerTarget = f;
    }

    void AnimateHand(){
        if (gripCurrent != gripTarget){
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * speed);
            animator.SetFloat(gripParam, gripCurrent);
        }
        if (triggerCurrent != triggerTarget){
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * speed);
            animator.SetFloat(triggerParam, triggerCurrent);
        }
    }

}
