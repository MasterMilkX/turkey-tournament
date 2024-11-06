using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance = null;

    // default to at least one player
    public int numPlayers = 1;
    public bool[] activatedPlayers = {true, false, false, false};

    // player colors
    public Color[] pColors = {new Color(0.82f, 0.30f, 0.30f,1f), 
                                new Color(0.20f, 0.32f, 1f,1f), 
                                new Color(1f, 0.85f, 0f, 1f), 
                                new Color(0.35f, 1f, 0f, 1f)};

    // selected game mode / map
    public int gameMode = 0;            // 0 = coop, 1 = versus, (2 = team; defected)
    public string[] allMaps = {"Map1", "Map2", "Map3"};
    public string gameMap = "Map1";     // default map

    // makes the game data persist between scenes
    void Awake() {
        // new copy
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(transform.gameObject);    // make this object persist between scenes
        }
        // otherwise destroy this copy
        else{
            Destroy(transform.gameObject);
        }
    }
}
