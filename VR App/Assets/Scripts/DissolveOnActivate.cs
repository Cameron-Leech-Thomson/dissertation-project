using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveOnActivate : MonoBehaviour, Activatable
{

    public Shader dissolveShader;

    bool shouldRemove = false;
    bool complete = false;

    bool active;
    
    List<float> times = new List<float>();
    List<Renderer> rends = new List<Renderer>();

    private string baseMap = "Texture2D_3c2299dec1d947c39196be3fe9e08d37";
    private string otherMap = "_MainTex";
    private string dissolveProgress = "_DissolveProgress";

    void Start() {
        Renderer[] rend = gameObject.GetComponents<Renderer>();
        foreach(Renderer r in rend){
            rends.Add(r);
            times.Add(0f);
        }
    }

    public void activate()
    {
        foreach (Renderer rend in rends){
            if (rend.enabled){
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Texture tex = rend.material.mainTexture;     
                Color col = rend.material.color;               
                
                rend.material.shader = dissolveShader;
                if (tex == null){
                    rend.material.color = col;
                } else{
                    mpb.SetTexture("_Texture", tex);
                    rend.SetPropertyBlock(mpb);
                }
                
                shouldRemove = true;
            }   
        }
    }

    public void deactivate(){}

    public bool isActive(){
        return active;
    }

    void Update() {
        if (shouldRemove && !complete){
            for(int i = 0; i < rends.Count; i++){
                float x = Mathf.Lerp(0f, 1f, times[i]);
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetFloat(dissolveProgress, x);
                rends[i].SetPropertyBlock(mpb);
                times[i] += Time.deltaTime;
            }
            if (Mathf.Lerp(0f, 1f, times[times.Count - 1]) == 1f){
                complete = true;
            }            
        }
        if (shouldRemove && complete){
            gameObject.SetActive(false);
        }
    }
}
