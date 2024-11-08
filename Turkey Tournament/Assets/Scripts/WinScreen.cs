using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    private GameData gameData;

    public Transform GUI;
    private Transform GUIPanel;

    // co-op mode
    private Text[] highscoreTexts;
    private Text statusText;

    public Transform[] playerSprites;




    // Start is called before the first frame update
    void Start(){
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        if(GUI != null){
            if(gameData.gameMode == 0){     // co-op mode
                GUIPanel = GUI.Find("CoopScreen");
                GUIPanel.gameObject.SetActive(true);
                GUI.Find("VSScreen").gameObject.SetActive(false);

                highscoreTexts = new Text[5];
                string[] places = {"1st", "2nd", "3rd", "4th", "5th"};

                for(int i = 0; i < 5; i++){
                    highscoreTexts[i] = GUIPanel.Find(places[i]+"Place").Find("Score").GetComponent<Text>();
                }

                // set the high scores if possible
                int scorePos = SetHighScores(gameData.teamScore);

                // set the status text
                statusText = GUIPanel.Find("ScoreStatus").GetComponent<Text>();
                if(scorePos != -1){
                    statusText.gameObject.SetActive(true);
                    if(scorePos == 0)
                        statusText.text = "New High Score! 1st place!";
                    else{
                        statusText.text = "Your team placed " + places[scorePos] + "!";
                    }
                } else {
                    statusText.gameObject.SetActive(false);
                }

                // set which player sprites based on the high score and position
                playerSprites = new Transform[4];
                Transform playerSection;
                if(scorePos == -1){
                    GUIPanel.Find("ScorePlayers").gameObject.SetActive(false);
                    playerSection = GUIPanel.Find("BottomPlayers");
                    playerSection.gameObject.SetActive(true);
                } else {
                    GUIPanel.Find("BottomPlayers").gameObject.SetActive(false);
                    playerSection = GUIPanel.Find("ScorePlayers");
                    playerSection.gameObject.SetActive(true);
                    Vector2 psPos = playerSection.position;
                    Vector2 rankPos = GUIPanel.Find((places[scorePos])+"Place").position;
                    playerSection.position = new Vector2(psPos.x, rankPos.y);
                }

                // set the player sprites
                for(int i = 0; i < 4; i++){
                    playerSprites[i] = playerSection.Find("p" + (i+1)+"Spr");
                    if(gameData.activatedPlayers[i]){
                        playerSprites[i].gameObject.SetActive(true);
                        playerSprites[i].GetComponent<Image>().color = gameData.pColors[i];
                    } else {
                        playerSprites[i].gameObject.SetActive(false);
                    }
                }
            }else if(gameData.gameMode == 1){   // versus mode
                GUIPanel = GUI.Find("VSScreen");
                GUIPanel.gameObject.SetActive(true);
                GUI.Find("CoopScreen").gameObject.SetActive(false);

                // set the player rankings
                int[] playerRanks = GetPlayerRankings();
                string[] places = {"1st", "2nd", "3rd", "4th"};

                Transform psprites = GUIPanel.Find("PlayerSprites");
                Transform losers = psprites.Find("Losers");


                for(int i = 0; i < 4; i++){
                    // first place player
                    if(playerRanks[i] == 1){
                        psprites.Find(places[0]+"Place").GetComponent<Image>().color = gameData.pColors[i];
                        GUIPanel.Find("WinnerText").GetComponent<Text>().text = "Player " + (i+1) + " Wins!";
                    } else{
                        losers.Find(places[playerRanks[i]-1]+"Place").GetComponent<Image>().color = gameData.pColors[i];
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update(){
        // if the z and m keys are held, reset the game to the player select screen
        if(Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.M)){
            gameData.legitGame = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerSelect");
        }
    }


    // retrieve the PlayerPrefs high scores
    public int[] GetHighScores(){
        int[] highScores = new int[5];
        for(int i = 0; i < 5; i++){
            if(PlayerPrefs.HasKey("HighScore" + i)){
                highScores[i] = PlayerPrefs.GetInt("HighScore" + i);
            } else {
                highScores[i] = 0;
                PlayerPrefs.SetInt("HighScore" + i, 0);
            }
        }
        return highScores;
    }

    // sets the high scores and returns true if in the top 5
    public int SetHighScores(int team_score){
        int[] highScores = GetHighScores();
        int score = team_score;
        int highPos = -1;
        for(int i = 0; i < 5; i++){
            if(score > highScores[i]){
                int temp = highScores[i];
                highScores[i] = score;
                score = temp;
                if(highPos == -1)
                    highPos = i;

            }
        }

        // change the text of the high scores
        for(int i = 0; i < 5; i++){
            highscoreTexts[i].text = highScores[i].ToString();
        }

        // save the new high scores (if a legit game)
        if(gameData.legitGame){
            for(int i = 0; i < 5; i++){
                PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
            }
        }

        return highPos;
    }

    public int[] GetPlayerRankings(){
        int[] playerRanks = new int[4];
        
        // determine the ranks of the player based on their scores
        for(int i = 0; i < 4; i++){
            int rank = 1;
            for(int j = 0; j < 4; j++){
                if(gameData.vsScores[i] < gameData.vsScores[j]){
                    rank++;
                }
            }
            playerRanks[i] = rank;
        }

        return playerRanks;
    }
}
