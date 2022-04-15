using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ButtonTrigger
{
    
    void OnCollisionEnter(Collision collisionInfo);

    void activateButton();

    void OnCollisionExit(Collision collisionInfo);

}
