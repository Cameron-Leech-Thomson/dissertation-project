using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition
{
    
    public static IEnumerator LerpPos(Transform obj, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = obj.position;
        while (time < duration)
        {
            obj.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        obj.position = targetPosition;
    }


}
