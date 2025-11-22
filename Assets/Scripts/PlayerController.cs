using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }

    bool isWalking, isGrounded = false; //Check player's current state (walking or jumping)

    Rigidbody2D playerRB; //player's rigidBody reference

    public float velocity; //Player's horizontal moving velocity

    public LayerMask checkGround; //Player's raycast is going to find ground tiles - ground tiles are in the checkGround Layer


    //Jumping variables
    public float apexTime, apexHeight;

    public float gravity, initialJumpVelocity;

    private bool jumpTrigger;

    //control player moving horizontally
    float xInput;

    Vector2 playerInput;

    //Jumping terminal speed
    public float terminalSpeed;

    //timer holds coyoteTime
    public float coyoteTime;
    float timer;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //reference, directly get


        //Set the gravity
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));

        //Set the initial jump velocity
        initialJumpVelocity = 2 * apexHeight / apexTime;
    }

    
    void Update()
    {
        //Check everyframe if there is ground tile underneath player
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, checkGround);
        //return true if there is a ground tile, means player is on the ground

        //Don't rotate the player when collides something
        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;
        transform.eulerAngles = rotation;


        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        //Vector2 playerInput = new Vector2();


        //check player input and use it as addForce's basic value
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticaltalInput = Input.GetAxisRaw("Vertical");
        playerInput = new Vector2(horizontalInput, 0);


        MovementUpdate(playerInput);



        //Check player whether is walking
        if (playerInput.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }


        //Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.down, Color.red);
        //Debug.Log(isGrounded);
    }





    private void FixedUpdate()
    {
        //playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;

        //player press space and within coyote time
        if (jumpTrigger)
        {
            //Give the jump velocity to player rigidbody
            playerRB.linearVelocityY = initialJumpVelocity;

            //Avoid player is receiving jump velocity everyframe - there is no code that sets jump trigger to false. Now set to false
            jumpTrigger = false;
        }

        if (isGrounded == false)
        {
            //Due to I have my own gravity, it should apply to player when they are not grounded
            playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;

            //The terminal speed makes sure that player's falling speed will not reach a giant amount
            if(playerRB.linearVelocity.y > terminalSpeed)
            {
                playerRB.linearVelocityY = terminalSpeed;
            }
        }
    }





    private void MovementUpdate(Vector2 playerInput)
    {
        //playerRB.AddForce(playerInput);
        //New player horizontal move:
        xInput = playerRB.linearVelocityX;
        xInput = playerInput.x * velocity;
        playerRB.linearVelocityX = xInput;

        //Coyote time
        //If player touches ground, reset coyote time
        if (isGrounded)
        {
            timer = coyoteTime;
        }
        else //if player has no ground tile below, start counting coyote time
        {
            timer -= Time.deltaTime;
        }

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded && coyoteTime < 0)
        if (Input.GetKeyDown(KeyCode.Space) && timer > 0) //coyote time now allows player can jump within this period of time. (rather than bool isGround)
        {
            jumpTrigger = true;
        }

    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }


    public FacingDirection GetFacingDirection()
    {
        FacingDirection direction; //Store player input's direction

        if (playerInput.x < 0)
        {
            direction = FacingDirection.left; //left when negative
        }
        else
        {
            direction = FacingDirection.right; //right when positive
        }
        return direction;
    }
}
