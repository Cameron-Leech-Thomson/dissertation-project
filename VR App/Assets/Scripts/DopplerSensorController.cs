using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.UIElements;

public class DopplerSensorController : MonoBehaviour
{

    [Tooltip("The game object(s) that will be activated once this button has been pressed")]
    public GameObject[] activates;

    public bool redShift = true;
    
    Color activatedColour;
    Color baseColour;
        
    LineRenderer lineRenderer;
    RenderLaser laser;
    RaycastHit raycastHit;

    private string specularID = "_SpecColor";
    private string lineColour = "_BaseColour";
    private string actColour = "_ActivateColour";

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        laser = GetComponent<RenderLaser>();
        baseColour = lineRenderer.material.GetColor(lineColour);
        activatedColour = lineRenderer.material.GetColor(actColour);
    }

    // Update is called once per frame
    void Update()
    {
        // If the laser has intersected with another object:
        RaycastHit hit = laser.raycastHit;
        if (Vector3.Distance(hit.point,laser.expectedFinish) > 0.05f && hit.point != Vector3.zero){
            // Get the component that collided with the ray:
            GameObject col = getGameObjectFromRay(hit);
            // Find the interactable component within the object:
            XRGrabInteractable interactable = col.GetComponentInParent<XRGrabInteractable>();
            GameObject obj;
            if (interactable != null){
                // Need to check object hue from doppler changes...
                // Get the renderers in each object:
                obj = interactable.gameObject;
                Renderer[] rends = obj.gameObject.GetComponentsInChildren<Renderer>();
                Color[] colours = new Color[rends.Length];
                for(int i = 0; i < rends.Length; i++){
                    // Get the MPB's for each renderer:
                    MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                    rends[i].GetPropertyBlock(mpb);
                    // Get the specular map from the material:
                    colours[i] = mpb.GetColor(specularID);

                    float h,s,v = 0;
                    Color.RGBToHSV(colours[i], out h, out s, out v);

                    // If the hue is tending to red:
                    if (h > (float)300f/360f && redShift){
                        Debug.Log("Red Shift");
                        activate();
                    } else if (h <= (float)300f/360f && h >= (float)240f/360f && !redShift){
                        // If the hue is tending to blue:
                        Debug.Log("Blue Shift");
                        activate();
                    }
                }
            }
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
        int numFlashes = 5;
        for (int i = 0; i < numFlashes; i++)
        {
            lineRenderer.material.SetColor(lineColour, activatedColour);
            yield return new WaitForSeconds(0.25f);
            lineRenderer.material.SetColor(lineColour, baseColour);
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
