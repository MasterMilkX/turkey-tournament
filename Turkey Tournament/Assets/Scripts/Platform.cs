using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public int platformType = 1;        // 1 - short, 2 - medium, 3 - long/floor, 0 - no spawn, 4 - invisible
    private Transform featherSpawn;     // reference to the feather spawn parent transform

    // Start is called before the first frame update
    void Start(){
        featherSpawn = GameObject.Find("FeatherSpawns").transform;  
        if(platformType != 0)
            AddSpawns();
    }


    // adds a feather spawn point above the platform height
    // varies based on the type
    void AddSpawns(){
        if(platformType == 1){
            GameObject newSpawn = new GameObject("fspawn_s");
            newSpawn.transform.parent = this.transform;
            newSpawn.transform.localPosition = new Vector2(0,0.35f);

            // add to the feather spawn list
            newSpawn.transform.parent = featherSpawn;
        }else if(platformType == 2){
            for(int i=0;i<3;i++){
                GameObject newSpawn = new GameObject("fspawn_m");
                newSpawn.transform.parent = this.transform;
                newSpawn.transform.localPosition = new Vector2((i*0.5f)-0.5f,0.25f);

                // add to the feather spawn list
                newSpawn.transform.parent = featherSpawn;
            }
        }else if(platformType == 3){
            for(int i=0;i<5;i++){
                GameObject newSpawn = new GameObject("fspawn_l");
                newSpawn.transform.parent = this.transform;
                newSpawn.transform.localPosition = new Vector2((i*0.5f)-1f,0.35f);

                // add to the feather spawn list
                newSpawn.transform.parent = featherSpawn;
            }
        }else if(platformType == 4){
            GameObject newSpawn = new GameObject("fspawn_i");
            newSpawn.transform.parent = this.transform;
            newSpawn.transform.localPosition = Vector2.zero;

            // add to the feather spawn list
            newSpawn.transform.parent = featherSpawn;
        }
    }
}
