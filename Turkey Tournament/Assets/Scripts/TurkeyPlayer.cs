using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sprite from
// <a href="https://www.flaticon.com/free-icons/turkey" title="turkey icons">Turkey icons created by Freepik - Flaticon</a>

public class TurkeyPlayer : MonoBehaviour
{
    // private variables
    private Rigidbody2D rb;
    private Transform bottom;
    private GameMasterScript master_script;
    private GameData gameData;
    private bool jumped;
    private Vector2 m_vel = Vector2.zero;
    private float move_damp = 0.1f;
    private bool canPlay = true;
    private float gravityVal;
    private SpriteRenderer sprRend;
    private Color baseColor;
    private Vector2 lastVel;

    [Header("General Properties")]
    public LayerMask levelMask;
    public bool isGrounded;
    public bool canBoost = true;
    private bool boosted = false;
    public float darkAmt = 0.25f;
    public Color boostColor = new Color(0.5f, 0.5f, 0.5f);


    [Header("Player Properties")]
    public int playerController = 0;
    public float moveSpeed = 10.0f;
    public float boostAmt = 2.0f;
    public float jumpForce = 5.0f;
    public float hor = 0.0f;

    [Header("Key Controls")]
    // TODO: Change these to Input Manager to also get in game key remapping
    public KeyCode leftBtn = KeyCode.A;
    public KeyCode rightBtn = KeyCode.D;
    public KeyCode jumpBtn = KeyCode.W;
    public KeyCode boostBtn = KeyCode.S;
    private bool useKeyboard = true;

    void Start(){
        rb = transform.GetComponent<Rigidbody2D>();
        sprRend = transform.GetComponent<SpriteRenderer>(); // get the sprite renderer
        baseColor = sprRend.color;
        //bottom = transform.Find("bottom");
        master_script = GameObject.Find("MasterController").GetComponent<GameMasterScript>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        canPlay = true;
        gravityVal = rb.gravityScale;

        if(playerController == 0){
            useKeyboard = true;
        }else{
            useKeyboard = false;
            sprRend.color = gameData.pColors[playerController-1];
        }
    }

    void Update() {
        //isGrounded = Grounded();        // check if the player is currently on the ground

        if(useKeyboard){
            // jump ability
            if(Input.GetKeyDown(jumpBtn))
                Fly();
            else if(Input.GetKeyUp(jumpBtn))
                jumped = false;

            // boost ability
            if(Input.GetKeyDown(boostBtn) && canBoost){
                StartCoroutine(Boost());
            }
        }else{
            // jump ability
            if(Input.GetButtonDown("Jump"+playerController.ToString()))
                Fly();
            else if(Input.GetButtonUp("Jump"+playerController.ToString()))
                jumped = false;

            // boost ability
            if(Input.GetButtonDown("Dash"+playerController.ToString()) && canBoost){
                StartCoroutine(Boost());
            }
        }
    }

    // for use with the movement only
    void FixedUpdate(){
         // detect horizontal control
        hor = 0;
        if(useKeyboard){
            if(Input.GetKey(leftBtn)){
                hor -= 1;
                sprRend.flipX = true;
            }if(Input.GetKey(rightBtn)){
                hor += 1;
                sprRend.flipX = false;
            }
        }else{
            hor = Input.GetAxis("Hor"+playerController.ToString());
            sprRend.flipX = hor < 0;
        }
        
        
        if(canPlay)
            Move(hor);
    }


    // moves the player in a specific direction based on the 'hor' input value (negative = left, positive = right)
    void Move(float hor){
        /*
            float boostVal = boosted ? boostAmt : 1.0f;
            float actHor = boosted ? lastHor : hor;
        */
        Vector2 new_vel = new Vector2(moveSpeed * hor * 20f * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, new_vel, ref m_vel, move_damp);
        if(boosted)
            rb.velocity = lastVel*boostAmt;
        else{
            lastVel = rb.velocity;
        }
    }

    // allows the player to jump/fly
    void Fly(){
        if(!jumped && canPlay){
            jumped = true;                                      // prevent extra force from being applied
            rb.AddForce(Vector2.up*jumpForce*rb.mass*100);
        }
    }   

    IEnumerator Boost(){
        boosted = true;
        canBoost = false;
        sprRend.color = boostColor;
        yield return new WaitForSeconds(0.3f);
        float gray = (baseColor.r + baseColor.g + baseColor.b) / 3;
        sprRend.color = new Color(gray,gray,gray);
        boosted = false;
        yield return new WaitForSeconds(2.0f);
        sprRend.color = baseColor;
        canBoost = true;
    }

    /*
    // check if the player is on the ground (uses a raycast detection)
    bool Grounded(){
        float distToGround = 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(bottom.position, -Vector2.up, distToGround, levelMask);
        return hit;
    }

    // resets the player at a specific position
    void Reset(Vector2 newPos){
        rb.velocity = Vector2.zero;
        transform.position = newPos;
        rb.gravityScale = gravityVal;
        canPlay = true;
    }

    // stop movement with the player totally
    public void StopAll(){
        Reset(transform.position);
        canPlay = false;
        rb.gravityScale = 0;
    }
    */
}
