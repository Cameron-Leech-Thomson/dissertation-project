using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorActivatable : MonoBehaviour, Activatable
{

    [Tooltip("The moving part of the door object")]
    public GameObject door;

    bool isActivated = false;

    Vector3 doorClosed;
    Vector3 doorOpen;

    // Start is called before the first frame update
    void Start()
    {
        // Get the door's original 'closed' position:
        doorClosed = door.transform.position;
        // Calculate the door's 'open' / 'down' position:
        doorOpen = door.transform.position - (new Vector3(0, door.transform.position.y * 2, 0));
    }

    public bool isActive(){
        return false;
    }

    public void activate(){
        // TODO: activate door.
        StartCoroutine(LerpPosition(doorOpen, 5));
        isActivated = true;
    }

    public void deactivate(){
        // TODO: deactivate door.
        StartCoroutine(LerpPosition(doorClosed, 5));
        isActivated = false;
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = door.transform.position;
        while (time < duration)
        {
            door.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        door.transform.position = targetPosition;
    }

}
