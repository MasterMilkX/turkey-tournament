using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sprite from
// <a href="https://www.flaticon.com/free-icons/feather" title="feather icons">Feather icons created by Freepik - Flaticon</a>


public class FeatherItem : MonoBehaviour
{
    private GameMasterScript master_script;

    // Start is called before the first frame update
    void Start(){
        master_script = GameObject.Find("MasterController").GetComponent<GameMasterScript>();
    }

    // when a turkey collides with the feather, add score and destroy the feather
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.CompareTag("turkey")){
            master_script.AddScore(col.gameObject.name);
            Destroy(gameObject);
        }
    }
}
