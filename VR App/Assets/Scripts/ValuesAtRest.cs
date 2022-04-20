using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ValuesAtRest : MonoBehaviour
{
    
    float restMass;
    Vector3 restLength;

    Renderer[] renderers;
    Color[] colours;

    MaterialPropertyBlock materialPropertyBlock;

    bool colourChanged = false;

    void Start() {
        materialPropertyBlock = new MaterialPropertyBlock();
        // Get rigidbody component:
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        // Get rest mass from Rigidbody vals:
        restMass = rb.mass;

        // Get relative size of the object from its starting transform:
        restLength = gameObject.transform.lossyScale;

        // Get the data from each material in the object:
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        colours = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            colours[i] = renderers[i].material.color;
        }
    }

    public bool isColourChanged(){
        return colourChanged;
    }

    // Get renderers:
    public Renderer[] getRenderers(){
        return renderers;
    }

    public void setColour(Color col){
        if (!materialPropertyBlock.isEmpty){
            materialPropertyBlock.Clear();
            // Set the colour to the MPB:
            materialPropertyBlock.SetColor("_BaseColor", col);
            // Apply the propertyBlock to the renderers:
            foreach(Renderer rend in renderers){
                rend.SetPropertyBlock(materialPropertyBlock);
            }
            materialPropertyBlock.Clear();
            colourChanged = true;
        }
    }

    public void resetColours(){
        if (!materialPropertyBlock.isEmpty){
            materialPropertyBlock.Clear();
            for (int i = 0; i < renderers.Length; i++)
            {
                // Set the colour to the MPB:
                materialPropertyBlock.SetColor("_BaseColor", colours[i]);
                // Apply the property to the renderer:
                renderers[i].SetPropertyBlock(materialPropertyBlock);
            }
            materialPropertyBlock.Clear();
            colourChanged = false;
        }
    }

    public Color[] getStartColours(){
        return colours;
    }

    public Vector3 getRestLength(){
        return restLength;
    }

    public float getRestMass(){
        return restMass;
    }

}
