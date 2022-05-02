using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{

    public GameObject nextLevel;
    public GameObject playerRig;

    Transform spawnpoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnpoint = nextLevel.transform.Find("Spawnpoint");
    }

    public void moveToNextLevel(){
        playerRig.transform.position = spawnpoint.position;
        playerRig.transform.eulerAngles = spawnpoint.eulerAngles;
    }


}
