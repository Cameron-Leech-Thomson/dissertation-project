using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ValuesAtRest : MonoBehaviour
{
    
    float restMass;
    Vector3 restLength;

    internal Vector3 restPos;
    Vector3 restRot;

    Renderer[] renderers;
    Color defaultSpecular = Color.HSVToRGB(0f, 0f, 0.2f);

    MaterialPropertyBlock materialPropertyBlock;

    Rigidbody rb;

    bool colourChanged = false;

    private string specularID = "_SpecColor";
    private string specularBool = "_SPECULARHIGHLIGHTS_OFF";
    private string specularHighlights = "_SpecularHighlights";

    void Start() {
        materialPropertyBlock = new MaterialPropertyBlock();
        // Get starting position of object in case it needs to be reset:
        restPos = gameObject.transform.position;
        restRot = gameObject.transform.eulerAngles;
        // Get rigidbody component:
        rb = gameObject.GetComponent<Rigidbody>();

        // Get rest mass from Rigidbody vals:
        restMass = rb.mass;

        // Get relative size of the object from its starting transform:
        restLength = gameObject.transform.lossyScale;

        // Get the data from each material in the object:
        renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer rend in renderers){
            rend.material.DisableKeyword(specularBool);
        }
    }

    public void resetPosition(){
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        gameObject.transform.position = restPos;
        gameObject.transform.eulerAngles = restRot;
    }

    public bool isColourChanged(){
        return colourChanged;
    }

    // Get renderers:
    public Renderer[] getRenderers(){
        return renderers;
    }

    public void setColour(Color col){
        if (materialPropertyBlock == null){
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        // Set the colour to the MPB:
        materialPropertyBlock.SetFloat(specularHighlights, 1f); 
        materialPropertyBlock.SetColor(specularID, col);
        float h, s, v = 0;
        Color.RGBToHSV(materialPropertyBlock.GetColor(specularID), out h, out s, out v);
        // Apply the propertyBlock to the renderers:
        foreach(Renderer rend in renderers){
            rend.SetPropertyBlock(materialPropertyBlock);
        }
        colourChanged = true;
    }

    public void resetColours(){
        if (materialPropertyBlock == null){
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        // Set the colour to the MPB:
        materialPropertyBlock.SetFloat(specularHighlights, 1f); 
        materialPropertyBlock.SetColor(specularID, defaultSpecular);
        float h, s, v = 0;
        Color.RGBToHSV(materialPropertyBlock.GetColor(specularID), out h, out s, out v);
        foreach(Renderer rend in renderers){
            // Apply the property to the renderer:
            rend.SetPropertyBlock(materialPropertyBlock);
        }
        colourChanged = false;
    }

    public Color getStartColour(){
        return defaultSpecular;
    }

    public Vector3 getRestLength(){
        return restLength;
    }

    public float getRestMass(){
        return restMass;
    }

}
