using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivePreviousObjects : MonoBehaviour
{

    public LayerMask playerLayer;
    
    public GameObject[] objectsToRemove;

    public Shader dissolveShader;

    bool shouldRemove = false;
    bool complete = false;
    
    List<float> times = new List<float>();
    List<Renderer> rends = new List<Renderer>();

    private string baseMap = "Texture2D_3c2299dec1d947c39196be3fe9e08d37";
    private string dissolveProgress = "_DissolveProgress";

    void Start() {
        foreach(GameObject obj in objectsToRemove){
            Renderer[] rend = obj.GetComponents<Renderer>();
            foreach(Renderer r in rend){
                rends.Add(r);
                times.Add(0f);
            }
        }
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        GameObject collision = collisionInfo.gameObject;
        if(LayerMask.GetMask(LayerMask.LayerToName(collision.layer)) == playerLayer){
            foreach (GameObject obj in objectsToRemove){
                Renderer rend = obj.GetComponent<Renderer>();
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Texture tex = rend.material.GetTexture(baseMap);
                rend.material.shader = dissolveShader;
                mpb.SetTexture("_Texture", tex);
                rend.SetPropertyBlock(mpb);
                shouldRemove = true;
            }
        }
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
                foreach(GameObject obj in objectsToRemove){
                    obj.SetActive(false);
                }
                complete = true;
            }            
        }
    }

}
