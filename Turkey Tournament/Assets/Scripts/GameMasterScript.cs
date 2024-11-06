using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMasterScript : MonoBehaviour
{

    [Header("Game Properties")]
    private GameData gameData;
    public int globalScore = 0;
    public int[] playerScore = {0, 0, 0, 0};
    public int maxCountDown = 100;
    public int timeCountDown = 100;

    public int startCountdown = 3;
    private Text countdownText;

    public string gameState = "Active";
    public int gameMode = 0;        // default co - op mode

    [Header("Player Properties")]
    public List<Transform> playerPos;
    public List<Transform> player;

    [Header("Item Properties")]
    public float maxFeatherSpawnTime = 1.75f;
    public float spawnTime = 1.75f;
    private float slope = 0;
    public Transform featherSpawns;         // feather spawn points parent transform
    public GameObject feather;              // feather prefab

    [Header("UI Properties")]
    public Transform GUI;
    private Transform GUIPanel;
    private UnityEngine.UI.Text scoreText;
    private List<UnityEngine.UI.Text> playerScoreText;
    private UnityEngine.UI.Text timeText;

    [Header("Key Controls")]
    // Keep these hardcoded so that a keyboard press can change it
    public KeyCode resetBtn = KeyCode.R;
    public KeyCode pauseBtn = KeyCode.Escape;

    // Start is called before the first frame update
    void Start(){
        
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameMode = gameData.gameMode;

        // activate the players and set their colors
        for(int i = 0; i < 4; i++){
            if(i < gameData.numPlayers){
                player[i].gameObject.SetActive(true);
                player[i].GetComponent<SpriteRenderer>().color = gameData.pColors[i];
                player[i].GetComponent<TurkeyPlayer>().playerController = i+1;
                player[i].position = playerPos[i].position;
            } else {
                player[i].gameObject.SetActive(false);
            }
        }

        // set the GUI
        if(GUI != null){
            // single player and coop mode have the same collaborative GUI
            if(gameMode == 0){
                GUIPanel = GUI.Find("CoopGUI");
                GUI.Find("VSGUI").gameObject.SetActive(false);
                scoreText = GUIPanel.Find("Score").GetComponent<UnityEngine.UI.Text>();
                timeText = GUIPanel.Find("Time").GetComponent<UnityEngine.UI.Text>();
            } 

            // versus mode has different GUI
            else{
                GUIPanel = GUI.Find("VSGUI");
                GUI.Find("CoopGUI").gameObject.SetActive(false);
                for(int i = 0; i < 4; i++){
                    if (i < gameData.numPlayers){
                        playerScoreText[i] = GUIPanel.Find("P" + (i+1) + "Score").GetComponent<UnityEngine.UI.Text>();
                        playerScoreText[i].gameObject.SetActive(true);
                        playerScoreText[i].color = gameData.pColors[i];
                    } else {
                        playerScoreText[i].gameObject.SetActive(false);
                    }
                }
            }

            countdownText = GUI.Find("Countdown").GetComponent<Text>();
            countdownText.gameObject.SetActive(true);
        }


        StartCoroutine(GameStart());
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetKeyDown(pauseBtn)){
            if(gameState == "Active"){
                PauseGame();
            } else if (gameState == "Paused"){
                ResumeGame();
            }
        }

        // reset the game
        if(Input.GetKeyDown(resetBtn)){
            ResetGame();
        }

        // check if the countdown is over
        if(timeCountDown <= 0 && gameState != "Game Over"){
            GameOver();
        }

        // update the GUI
        UpdateGUI();
    }   



    /// =============   GAME CONTROLS  ============= ///

    // completely resets the game
    public void ResetGame(){
        // reset the player positions
        for(int i = 0; i < player.Count; i++){
            player[i].position = playerPos[i].position;
        }

        // reset the scores
        globalScore = 0;
        playerScore = new int[]{0, 0, 0, 0};

        // reset the countdown
        timeCountDown = maxCountDown;
        StartCoroutine(CountDown());        // start the countdown


        // reset the feathers
        Transform[] allFeathSpawns = featherSpawns.GetComponentsInChildren<Transform>();
        for(int i = 0; i < allFeathSpawns.Length; i++){
            if(allFeathSpawns[i].childCount > 0){
                Destroy(allFeathSpawns[i].GetChild(0).gameObject);
            }
        }
        StartCoroutine(SpawnFeathers());    // start spawning feathers

        ResumeGame();       // resume the game
    }

    public void PauseGame(){
        // pause the game
        Time.timeScale = 0;
        gameState = "Paused";
    }

    public void ResumeGame(){
        // resume the game
        Time.timeScale = 1;
        gameState = "Active";
    }

    // countdown timer
    IEnumerator CountDown(){
        while(timeCountDown > 0){
            yield return new WaitForSeconds(1);
            timeCountDown -= 1;
        }
    }

    IEnumerator GameStart(){
        while(startCountdown > 0){
            countdownText.text = startCountdown.ToString();
            yield return new WaitForSeconds(1);
            startCountdown -= 1;
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);
        ResetGame();
    }

    public void GameOver(){
        // end the game
        Time.timeScale = 0;
        gameState = "Game Over";
        Debug.Log("Game Over");
        StopAllCoroutines();
    }

    /// =============   SCORING  ============= ///
    
    public void AddScore(String player){
        if(gameMode == 0){
            globalScore += 1;           // increase the global score
        }else if(gameMode == 1){
            int player_num = Int32.Parse(player.Substring(player.Length - 1))-1;
            playerScore[player_num] += 1;       // increase the player's score
        }
    }

       

    /// =============   FEATHER SPAWNING  ============= ///
    
    
    // spawn feathers at random intervals in random positions
    IEnumerator SpawnFeathers(){
        while(true){
            if(timeCountDown <= 0) break;       // stop spawning feathers if the game is over
            spawnTime = maxFeatherSpawnTime;
            
            if(timeCountDown > (maxCountDown/2)){
                spawnTime = Math.Max(0.5f,(float)(maxFeatherSpawnTime * Math.Log(maxCountDown-(maxCountDown-timeCountDown), maxCountDown)));
            }else{
                slope = (0-maxFeatherSpawnTime) / maxCountDown;
                spawnTime = (slope * (maxCountDown - timeCountDown)) + maxFeatherSpawnTime;
            }
            
            yield return new WaitForSeconds(spawnTime);
            AddFeather();
        }
    }

    // add a feather to the game
    public void AddFeather(){
        // get the spawn positions without feathers
        List<Transform> emptySpawns = new List<Transform>();
        Transform[] allFeathSpawns = featherSpawns.GetComponentsInChildren<Transform>();
        for(int i = 0; i < allFeathSpawns.Length; i++){
            if(allFeathSpawns[i].childCount == 0){
                emptySpawns.Add(allFeathSpawns[i]);
            }
        }
        if (emptySpawns.Count == 0) return; // no empty spawns

        // randomly select a spawn point
        int rand = UnityEngine.Random.Range(0, emptySpawns.Count);
        Transform spawn = emptySpawns[rand];

        // instantiate the feather
        GameObject newFeather = Instantiate(feather, spawn.position, Quaternion.identity);
        newFeather.transform.parent = spawn;
    }




    /// =============   GUI  ============= ///
    
    // update the GUI
    public void UpdateGUI(){
        if(gameMode == 0){
            scoreText.text = "Score: " + globalScore;
        } else if(gameMode == 1){
            for(int i = 0; i < 4; i++){
                if (i < gameData.numPlayers){
                    playerScoreText[i].text = "P" + (i+1) + " Score: " + playerScore[i];
                }
            }
        }

        timeText.text = "Time: " + timeCountDown;
    }
}
