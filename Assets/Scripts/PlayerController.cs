using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //In which direction that player is currently facing, changed by direction keys input
    public enum FacingDirection
    {
        left, right
    }

    //Player's movement state, updating animation
    public enum CharacterState
    {
        idle, walk, jump, death
    }

    //Initiallize player animation's currentState and previousState. Starting from no moving so that it's idle state.
    public CharacterState currentState = CharacterState.idle;
    public CharacterState previousState = CharacterState.idle;

    bool isWalking, isGrounded = false; //Check player's current motion state (walking or jumping)

    Rigidbody2D playerRB; //player's rigidBody reference

    //acclerationTime decides how long acclerate to reach the maxSpeed. decelerationTime goes to zero speed.
    public float maxSpeed;
    public float accelerationTime, decelerationTime;

    //current velocity holds current player's motion depend on rigidBody
    private Vector3 currentVelocity;
    private float acceleration, deceleration;

    
    public float health;
    public bool hasDied;



    public LayerMask checkGround; //Player's raycast is going to find ground tiles - ground tiles are in the checkGround Layer




    //Jumping variables
    public float apexTime, apexHeight;

    public float gravity, initialJumpVelocity;
    float initialGravity; //give gravity back when dashing has finished (I set gravity to zero when dashing)

    private bool jumpTrigger;



    //tracks player's horizontal input
    Vector2 playerInput;


    //Jumping terminal speed
    public float terminalSpeed;

    //timer holds coyoteTime
    public float coyoteTime;
    float timer;


    //Dashing variables
    public float dashSpeed;
    bool dashTrigger = false;
    bool canDash = true;
    bool afterFirstDash = false; //when isGrounded, set canDash to true. But at the beginning player is already isGrounded.
                                 //So this bool doesn't allow player dash at the beginning.






    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //reference, directly get


        //Set the gravity
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialGravity = gravity; //assign to gravity when dashing coroutine finished

        //Set the initial jump velocity
        initialJumpVelocity = 2 * apexHeight / apexTime;

        //Calculate acceleration & deceleration amount
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
    }

    
    void Update()
    {
        //Check everyframe if there is ground tile underneath player
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.95f, checkGround);
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

            currentVelocity.y = playerRB.linearVelocityY;

            //Avoid player is receiving jump velocity everyframe - there is no code that sets jump trigger to false. Now set to false
            jumpTrigger = false;
        }

        if (isGrounded == false)
        {
            //Due to I have my own gravity, it should apply to player when they are not grounded
            playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;
            
            currentVelocity.y = playerRB.linearVelocityY;

            //The terminal speed makes sure that player's falling speed will not reach a giant amount
            if (currentVelocity.y > terminalSpeed)
            {
                currentVelocity.y = terminalSpeed;
            }

            //The terminal speed makes sure that player's falling speed will not reach a giant amount
            //if (playerRB.linearVelocity.y > terminalSpeed)
            //{
            //    playerRB.linearVelocityY = terminalSpeed;
            //}

            //Doing dash
            if (dashTrigger)
            {
                dashTrigger = false;
                StartCoroutine(IsDashing());
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
        if (playerInput.x != 0)
        {
            isWalking = true;

            //horizontal movement
            currentVelocity += playerInput.x * acceleration * Vector3.right * Time.deltaTime;
            //Do not want velocity bigger than maxSpeed
            if(Mathf.Abs(currentVelocity.x) > maxSpeed)
            {
                currentVelocity = new Vector3(Mathf.Sign(currentVelocity.x) * maxSpeed, currentVelocity.y);
            }
        }
        else //Decelerate player
        {
            isWalking = false;

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

        playerRB.linearVelocityX = currentVelocity.x;
        //Debug.Log(currentVelocity);



        //My previous horizontal motion
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

        //Due to I only allow player dashing in the air, so isGrounded should be false; there should be a direction-to-move, so there has to be a playerInput value as well.
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isGrounded == false && playerInput.x != 0)
        {
            dashTrigger = true;
        }

        //After first dash, once player gets ground, allow they dash again
        if (afterFirstDash && isGrounded)
        {
            canDash = true;
        }

    }

    //If health is less than 0, player is dead, playering death animation
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

    //Dashing coroutine -> fixedUpdate
    IEnumerator IsDashing()
    {
        //Do not dashing again before touching ground
        canDash = false;

        float t = 0; //timer -> tracks dashing duration
        float direction = Mathf.Sign(playerInput.x); //Get current player's facing direction

        //currentVelocity.x = direction * dashSpeed;

        gravity = 0; //Set gravity to 0 when dashing

        //player dashing for 1 second
        while (t < 1)
        {
            currentVelocity.x = direction * dashSpeed; //calculate dashing velocity
            playerRB.linearVelocity = currentVelocity; //apply dashing velocity
            t += Time.fixedDeltaTime;
            yield return null;
        }

        gravity = initialGravity; //Set gravity back when finished dashing

        afterFirstDash = true;
    }
}
