using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;

public class DopplerSensorController : MonoBehaviour
{
    public bool redShift = true;

    [Tooltip("The game object(s) that will be activated once this button has been pressed")]
    public GameObject[] activates;
    
    Color activatedColour;
    Color baseColour;
        
    LineRenderer lineRenderer;
    RenderLaser laser;
    CapsuleCollider collider;
    RaycastHit raycastHit;

    bool activated = false;

    private string specularID = "_SpecColor";
    private string lineColour = "_BaseColour";
    private string actColour = "_ActivateColour";

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        laser = GetComponent<RenderLaser>();
        collider = GetComponent<CapsuleCollider>();
        baseColour = lineRenderer.material.GetColor(lineColour);
        activatedColour = lineRenderer.material.GetColor(actColour);
        setupCollider(laser.laserMaxLength, laser.expectedFinish);
    }

    void setupCollider(float length, Vector3 endpoint){
        collider.height = length;
        collider.center = midpoint(transform.InverseTransformPoint(transform.position), transform.InverseTransformPoint(endpoint));
    }

    Vector3 midpoint(Vector3 v1, Vector3 v2){
        Vector3 mp = Vector3.zero;
        mp.x = (v1.x + v2.x) / 2;
        mp.y = (v1.y + v2.y) / 2;
        mp.z = (v1.z + v2.z) / 2;
        return mp;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!activated){
            // Get the component that collided with the beam:
            GameObject collision = other.gameObject;
            // Find the interactable component within the object:
            XRBaseInteractable interactable = collision.GetComponentInParent<XRBaseInteractable>();
            if (interactable != null){
                // Get the game object:
                GameObject obj = interactable.gameObject;
                // Get the renderers in each object:
                Renderer[] rends = obj.gameObject.GetComponentsInChildren<Renderer>();
                Color[] colours = new Color[rends.Length];
                for (int i = 0; i < rends.Length; i++)
                {
                    // Get the MPB's for each renderer:
                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    rends[i].GetPropertyBlock(mpb);
                    // Get the specular map from the material:
                    colours[i] = mpb.GetColor(specularID);

                    float h, s, v = 0;
                    Color.RGBToHSV(colours[i], out h, out s, out v);

                    // If the hue is tending to red:
                    if (h > (float)300f / 360f && redShift)
                    {
                        activate();
                    }
                    else if (h <= (float)300f / 360f && h >= (float)240f / 360f && !redShift)
                    {
                        // If the hue is tending to blue:
                        activate();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the laser has intersected with another object:
        RaycastHit hit = laser.raycastHit;
        Vector3 point = hit.point;
        if (Vector3.Distance(point,laser.expectedFinish) > 0.05f && hit.point != Vector3.zero){
            // Move the collider to fit that:
            setupCollider(Vector3.Distance(transform.position, point), point);
        } if ((Vector3.Distance(point,laser.expectedFinish) < 0.05f || hit.point == Vector3.zero) && collider.height < laser.laserMaxLength){
            setupCollider(laser.laserMaxLength, laser.expectedFinish);
        }
    }

    void activate(){
        StartCoroutine(flashColour());
        // Activate the attached components:
        foreach (GameObject obj in activates){
			// Get the activator component of the object:
            Activatable activator = obj.GetComponent<Activatable>();
			if (activator != null){
				activator.activate();
			}
		}
    }

    private IEnumerator flashColour(){
        int numFlashes = 6;
        for (int i = 0; i < numFlashes; i++)
        {
            lineRenderer.material.SetColor(lineColour, baseColour);
            yield return new WaitForSeconds(0.25f);
            lineRenderer.material.SetColor(lineColour, activatedColour);
            yield return new WaitForSeconds(0.25f);
        }
    }

    GameObject getGameObjectFromRay(RaycastHit hit){
        if (hit.collider != null){
            return hit.collider.gameObject;
        } else if (hit.rigidbody != null){
            return hit.rigidbody.gameObject;
        } else {
            return null;
        }
    }

}
