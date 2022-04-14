using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base for any object that will be activated by a trigger event.
public interface Activatable
{

    // Get the current state of the object:
    bool isActive();

    // Causes the activatable object to shift from it's deactivated state to its activated state.
    void activate();

    // Reverts the object back to its prior state.
    void deactivate();

}
