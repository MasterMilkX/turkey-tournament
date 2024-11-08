using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    private GameData gameData;

    public Transform GUI;
    private Transform[] playerSprites; 

    public int selectedMap = 0;
    private Image[] mapImages;
    private bool changedMap = false;

    public int countdown = 20;  // countdown timer
    private Transform countdownText;    // countdown text


    // Start is called before the first frame update
    void Start(){
        gameData = GameObject.Find("GameData").GetComponent<GameData>();

        if(GUI != null){

            countdownText = GUI.Find("Countdown");

            mapImages = new Image[GUI.Find("MapImages").childCount];
            for(int i = 0; i < mapImages.Length; i++){
                mapImages[i] = GUI.Find("MapImages").GetChild(i).GetComponent<Image>();
            }

            // get all the children of PlayerSprites 
            playerSprites = new Transform[GUI.Find("PlayerSprites").childCount];
            for(int i = 0; i < playerSprites.Length; i++){
                playerSprites[i] = GUI.Find("PlayerSprites").GetChild(i);
            }
            
            AddPlayerSprites();     // add the turkeys at the bottom
            ChangeMap();           // change the mode to the first one
            StartCoroutine(CountdownScreen());      // start the countdown
        }
    }



    // Update is called once per frame
    void Update(){
        // player 1 selects the game mode 
        // TODO: Change this to controller input
       if(Input.GetAxis("Hor1") < 0 && !changedMap){
            selectedMap -= 1;
            if(selectedMap < 0){
                selectedMap = 2;
            }
            ChangeMap();
        }
        else if(Input.GetAxis("Hor1") > 0 && !changedMap){
            selectedMap += 1;
            if(selectedMap > 2){
                selectedMap = 0;
            }
            ChangeMap();
        }else if(Input.GetAxis("Hor1") == 0){
            changedMap = false;
        }
        
        if(Input.GetButtonDown("Start1")){
            SelectMap();
        }

        for(int i = 0; i < 4; i++){
            // if the player has already joined, allow them to hop
            if(gameData.activatedPlayers[i]){
                if(AnyButton(i+1)){
                    StartCoroutine(PlayerHop(i+1));
                }
            }
        }
    }

    // add player sprites based on how many players are active
    void AddPlayerSprites(){

        for(int i = 0; i < 4; i++){
            if(gameData.activatedPlayers[i]){
                playerSprites[i].gameObject.SetActive(true);
                playerSprites[i].GetComponent<Image>().color = gameData.pColors[i];
            }else{
                playerSprites[i].gameObject.SetActive(false);
            }
        }

        // shift the player sprites to the right for every fewer player
        Transform p = GUI.Find("PlayerSprites");
        Vector2 pspr_pos = p.localPosition;
        Debug.Log(pspr_pos);
        p.localPosition = new Vector2(pspr_pos.x + (4-gameData.numPlayers)*100, pspr_pos.y);
    }

    void ChangeMap(){
        for(int i = 0; i < mapImages.Length; i++){
            mapImages[i].color = new Color(0.5f,0.5f,0.5f,1f);
            mapImages[i].transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
        }
        mapImages[selectedMap].color = new Color(1f, 1f, 1f, 1f);
        mapImages[selectedMap].transform.localScale = new Vector3(1f, 1f, 1f);

        changedMap = true;
    }

    void SelectMap(){
        gameData.gameMap = gameData.allMaps[selectedMap];
        Debug.Log("Game Map: " + selectedMap);

        // load the next scene based on the map selection
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameData.allMaps[selectedMap]);

        // use the demo for now
        //UnityEngine.SceneManagement.SceneManager.LoadScene("DemoLevel");
    }

    // Countdown the player selection screen before moving to the next scene
    IEnumerator CountdownScreen(){
        for(int i = countdown; i > 0; i--){
            yield return new WaitForSeconds(1);
            countdownText.GetComponent<Text>().text = i.ToString();
        }
        
        // if the countdown is over, select the game mode to the current selection
        SelectMap();
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

    bool AnyButton(int playerNum){
        return Input.GetButtonDown("Start"+playerNum.ToString()) || Input.GetButtonDown("Jump"+playerNum.ToString()) || Input.GetButtonDown("Dash"+playerNum.ToString());
    }

}
