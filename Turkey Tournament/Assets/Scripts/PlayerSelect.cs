using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor; // Removed unnecessary using directive
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    private GameData gameData;

    public Sprite turkeySprite;
    public Sprite questionSprite;

    public Transform GUI;       // GUI parent transform
    private Transform[] playerSprites;  // player sprites
    private Transform[] playerStatus;   // player status
    private int curPlayer = 0;           // current number of players

    public int countdown = 30;  // countdown timer
    private Transform countdownText;    // countdown text

    public Color[] pColors;     // player colors

    // Start is called before the first frame update
    void Start(){
        
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameData.NewRound();        // reset the game data for a new round starting with this screen

        pColors = gameData.pColors;

        if(GUI != null){
            // get all the children of PlayerSprites 
            playerSprites = new Transform[GUI.Find("PlayerSprites").childCount];
            for(int i = 0; i < playerSprites.Length; i++){
                playerSprites[i] = GUI.Find("PlayerSprites").GetChild(i);
            }

            // get all the children of PlayerStatus
            playerStatus = new Transform[GUI.Find("PlayerStatus").childCount];
            for(int i = 0; i < playerStatus.Length; i++){
                playerStatus[i] = GUI.Find("PlayerStatus").GetChild(i);
            }

            // get the countdown text
            countdownText = GUI.Find("Countdown");
            countdownText.GetComponent<Text>().text = countdown.ToString();

            ResetAllPlayers();
            StartCoroutine(CountdownScreen());
        }
    }

    // Update is called once per frame
    void Update(){
        // check if any player has pressed the space key ( TODO: Change this to controller input)
        /*
        if(Input.GetKeyDown(KeyCode.Space) && curPlayer < 4){
            ActivatePlayer();
        }
        */
        for(int i = 0; i < 4; i++){
            if(Input.GetButtonDown("Start"+(i+1).ToString()) && !gameData.activatedPlayers[i]){
                ActivatePlayer(i);
            }

            // if the player has already joined, allow them to hop
            if(gameData.activatedPlayers[i]){
                if(AnyButton(i+1)){
                    StartCoroutine(PlayerHop(i+1));
                }
            }

        }

    }

    // Makes all the players start with unknown until they are activated with the controllers
    void ResetAllPlayers(){
        // change all the sprites to question mark and the status to "Press Start to Join"
        for(int i = 0; i < playerSprites.Length; i++){
            Image playerImage = playerSprites[i].GetComponent<Image>();
            Text playerText = playerStatus[i].GetComponent<Text>();
            
            playerImage.sprite = questionSprite;
            playerImage.color = pColors[i];
            playerText.text = "Press Start to Join";
            playerText.fontSize = 32;
            playerText.color = new Color(255, 255, 255);
        }

         // reset the game data for the players
        gameData.numPlayers = 0;
        gameData.activatedPlayers = new bool[4];
        for(int i = 0; i < 4; i++){
            gameData.activatedPlayers[i] = false;
        }
    }

    // activates the player based on number
    void ActivatePlayer(int playerNum){
        // change the sprite to the turkey and the status to "Joined"
        playerSprites[playerNum].GetComponent<Image>().sprite = turkeySprite;
        Text status = playerStatus[playerNum].GetComponent<Text>();
        status.text = "READY!";
        status.fontSize = 48;
        status.color = new Color(255, 188, 0);
        curPlayer++;
        gameData.numPlayers = curPlayer;
        gameData.activatedPlayers[playerNum] = true;
    }

    bool AnyButton(int playerNum){
        return Input.GetButtonDown("Start"+playerNum.ToString()) || Input.GetButtonDown("Jump"+playerNum.ToString()) || Input.GetButtonDown("Dash"+playerNum.ToString());
    }

    // Countdown the player selection screen before moving to the next scene
    IEnumerator CountdownScreen(){
        for(int i = countdown; i > 0; i--){
            yield return new WaitForSeconds(1);
            countdownText.GetComponent<Text>().text = i.ToString();

            if(curPlayer == 4 && i > 5){
                i = 5;  // skip the countdown if all players have joined 
            }
        }
        
        //

        // reset if there are no players
        if(curPlayer == 0){
            ResetAllPlayers();
            StartCoroutine(CountdownScreen());
        }
        // otherwise, move to the next scene
        else{
            if(curPlayer == 1)
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelect");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameModeSelect");
            //Debug.Log("GOTO: GameModeSelect");
        }
    }

    // make the player hop animation
    IEnumerator PlayerHop(int playerNum){
        // make the player hop
        Vector2 curPos = playerSprites[playerNum-1].transform.position;
        playerSprites[playerNum-1].transform.position = new Vector2(curPos.x, curPos.y + 0.2f);
        // play gobble sound effect
        yield return new WaitForSeconds(0.1f);
        playerSprites[playerNum-1].transform.position = curPos;
        
    }
}
