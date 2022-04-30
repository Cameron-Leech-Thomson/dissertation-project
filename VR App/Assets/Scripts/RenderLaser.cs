using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLaser : MonoBehaviour
{

    public float laserMaxLength = 5f;
    public LayerMask ignore;
    bool readyToFire = false;
    LineRenderer lineRenderer;
    RenderWireframe wireframe;

    internal RaycastHit raycastHit;
    internal Vector3 expectedFinish;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        wireframe = GetComponent<RenderWireframe>();

        expectedFinish = transform.position + (transform.up * laserMaxLength);

        StartCoroutine(waitForStart());
    }

    private IEnumerator waitForStart(){
        // Don't set up the line renderer until RenderWireframe.Start() is complete:
        while(!wireframe.isStarted){
            yield return null;
        }
        lineRenderer.positionCount += 2;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, transform.position);
        readyToFire = true;
    }

    void FixedUpdate() {
        if (lineRenderer.enabled && readyToFire){
            fireLaserForward(transform.position);
        }
    }

    void fireLaserForward(Vector3 origin){
        // Set up ray for set distance forward:
        Ray ray = new Ray(origin, transform.up);
        Vector3 finishPos = origin + (transform.up * laserMaxLength);

        if(Physics.Raycast(ray, out raycastHit, laserMaxLength, ~ignore)){
            finishPos = raycastHit.point;
        }

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, finishPos);
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, origin);
    }

    Vector3 vectorAbs(Vector3 v){
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        v.z = Mathf.Abs(v.z);
        return v;
    }

}
