using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }

    public enum CharacterState
    {
        idle, walk, jump, death
    }

    public CharacterState currentState = CharacterState.idle;
    public CharacterState previousState = CharacterState.idle;

    bool isWalking, isGrounded = false; //Check player's current state (walking or jumping)

    Rigidbody2D playerRB; //player's rigidBody reference

    public float maxSpeed;
    public float accelerationTime, decelerationTime;

    private Vector3 currentVelocity;
    private float acceleration, deceleration;

    public float health;
    public bool hasDied;

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

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
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

        HasDied();

        //Check player whether is walking
        if (playerInput.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        StateUpdate();
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

    private void StateUpdate()
    {
        previousState = currentState;

        //What is our current state?
        if (IsWalking() && IsGrounded())
        {
            currentState = CharacterState.walk;
        }
        else if (!IsGrounded())
        {
            currentState = CharacterState.jump;
        }
        else
        {
            currentState = CharacterState.idle;
        }

        if (health <= 0)
        {
            currentState = CharacterState.death;
        }
    }



    private void MovementUpdate(Vector2 playerInput)
    {
        if(playerInput.x != 0)
        {
            currentVelocity += playerInput.x * acceleration * Vector3.right * Time.deltaTime;
            if(Mathf.Abs(currentVelocity.x) > maxSpeed)
            {
                currentVelocity = new Vector3(Mathf.Sign(currentVelocity.x) * maxSpeed, currentVelocity.x);
            }
        }
        else
        {
            //If there's jump logic, using currentVelocity will cause issues here as the y will contributes to the velocity
            Vector3 amountWantToChange = deceleration * currentVelocity.normalized * Time.deltaTime;

            if(amountWantToChange.magnitude > Mathf.Abs(currentVelocity.x))
            {
                currentVelocity.x = 0;
            }
            else
            {
                currentVelocity -= amountWantToChange;
            }
        }

        playerRB.linearVelocity = currentVelocity;


        //playerRB.AddForce(playerInput);
        //New player horizontal move:
        //xInput = playerRB.linearVelocityX;
        //xInput = playerInput.x * velocity;
        //playerRB.linearVelocityX = xInput;

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

    public bool HasDied()
    {
        bool isDead = health <= 0;
        if (isDead && hasDied == false)
        {
            hasDied = true;
            return true;
        }
        return false;
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
