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

    // final scores
    public int[] vsScores = {0, 0, 0, 0};
    public int teamScore = 0;

    public bool legitGame = false;      // whether to save the scores to the leaderboard

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

    // resets the game data for a new round
    public void NewRound(){
        vsScores = new int[] {0, 0, 0, 0};
        teamScore = 0;
        gameMap = allMaps[Random.Range(0, allMaps.Length)];
        gameMode = 0;
        numPlayers = 1;
        activatedPlayers = new bool[]{true, false, false, false};
        legitGame = true;
    }

}
