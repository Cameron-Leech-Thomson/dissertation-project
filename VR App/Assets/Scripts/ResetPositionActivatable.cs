using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionActivatable : MonoBehaviour, Activatable
{

    public Shader dissolveShader;
    [Tooltip("Whether the script should be activated by any trigger, or only programmatically")]
    public bool activateAnyTrigger = true;

    bool shouldRemove = false;
    bool complete = false;

    bool activated = false;

    ValuesAtRest vals = null;

    List<float> times = new List<float>();
    List<Renderer> rends = new List<Renderer>();
    List<Shader> shaders = new List<Shader>();

    private string baseMap = "Texture2D_3c2299dec1d947c39196be3fe9e08d37";
    private string dissolveProgress = "_DissolveProgress";

    void Start() {
        Renderer[] rend = GetComponents<Renderer>();
        foreach(Renderer r in rend){
            rends.Add(r);
            shaders.Add(r.material.shader);
            times.Add(0f);
        }            
    }

    void Update()
    {
        if(vals == null){
            vals = gameObject.GetComponent<ValuesAtRest>();
        }
        if (shouldRemove){
            for(int i = 0; i < rends.Count; i++){
                float x = Mathf.Lerp(0f, 1f, times[i]);
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetFloat(dissolveProgress, x);
                rends[i].SetPropertyBlock(mpb);
                times[i] += Time.deltaTime;
            }
            if (Mathf.Lerp(0f, 1f, times[times.Count - 1]) == 1f){
                vals.resetPosition();
                resetShaders();
                shouldRemove = false;
            }
        }
    }

    void resetShaders(){
        for(int i = 0; i < rends.Count; i++){
            rends[i].material.shader = shaders[i];
            times[i] = 0f;
        }
    }

    public void activate(){
        foreach (Renderer rend in rends){
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Texture tex = rend.material.GetTexture(baseMap);
            rend.material.shader = dissolveShader;
            mpb.SetTexture("_Texture", tex);
            rend.SetPropertyBlock(mpb);
            shouldRemove = true;
        }
    }

    public void deactivate(){
        
    }

    public bool isActive(){
        return activated;
    }

}
