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
        idle, walk, jump, smash, dash, death
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


    //player's death check
    public float health;
    public bool hasDied;



    public LayerMask checkGround; //Player's raycast is going to find ground tiles - ground tiles are in the checkGround Layer




    //Jumping variables
    public float apexTime, apexHeight;

    public float gravity, initialJumpVelocity;
    float initialGravity; //give gravity back when dashing has finished (I set gravity to zero when dashing)
    float originalInitialJumpVelocity; //Due to player's jump can be charged, I have to save the original jump velocity.

    private bool jumpTrigger;
    //Jump charging variables
    bool isCharging;
    float chargingTime;


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
    bool isDashing = false; //for dashing animation
    bool afterFirstDash = false; //when isGrounded, set canDash to true. But at the beginning player is already isGrounded.
                                 //So this bool doesn't allow player dash at the beginning.

    //Smashing variables
    public float smashSpeed;
    bool smashTrigger = false;
    bool canSmash = true;
    bool isSmashing = false; //for smashing animation



    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //reference, directly get


        //Set the gravity
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialGravity = gravity; //assign to gravity when dashing coroutine finished

        //Set the initial jump velocity
        initialJumpVelocity = 2 * apexHeight / apexTime;
        originalInitialJumpVelocity = initialJumpVelocity; //assign to initialJumpVelocity after each charged jump

        //Calculate acceleration & deceleration amount
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;

    }

    
    void Update()
    {
        //Check everyframe if there is ground tile underneath player
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 2.5f, checkGround);
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
        ///////////////////////////////////////APPLYING JUMP VELOCITY & GRAVITY////////////////////////////////////////////////////////////////////
        
        //playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;

        //player press space and within coyote time
        if (jumpTrigger)
        {
            //Give the jump velocity to player rigidbody
            playerRB.linearVelocityY = initialJumpVelocity;

            currentVelocity.y = playerRB.linearVelocityY;

            //Avoid player is receiving jump velocity everyframe - there is no code that sets jump trigger to false. Now set to false
            jumpTrigger = false;

            initialJumpVelocity = originalInitialJumpVelocity; //Get back original jump velocity (charging jump changed it)
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

         /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            //Doing dash
            if (dashTrigger)
            {
                dashTrigger = false;
                StartCoroutine(IsDashing());
            }

            //Doing smash
            if (smashTrigger)
            {
                smashTrigger = false;
                StartCoroutine(DownWardSmash());
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

        if (isDashing)
        {
            currentState = CharacterState.dash;
        }

        if (isSmashing)
        {
            currentState = CharacterState.smash;
        }

        if (health <= 0)
        {
            currentState = CharacterState.death;
        }
    }





    private void MovementUpdate(Vector2 playerInput)
    {
        ///////////////////////////////////////HORIZONTAL MOVEMENT////////////////////////////////////////////////////////////////////////
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






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






        ///////////////////////////////////////CHARGING JUMP//////////////////////////////////////////////////////////////////////////////
        Debug.Log(chargingTime);
        //When get SpaceBar key's input, start charging; I also apply coyote time
        if (Input.GetKeyDown(KeyCode.Space) && timer > 0) //coyote time now allows player can jump within this period of time. (rather than bool isGround)
        {
            isCharging = true;
            chargingTime = 0;
        }
        //Calculating time during charging
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            chargingTime += Time.deltaTime;
        }
        //if time is less than 0.2 second, it's like "mouseButtonDown", so it's a normal jump
        if (Input.GetKeyUp(KeyCode.Space) && chargingTime < 0.2f && isCharging)
        {
            isCharging = false; //Stop charging when space bar is released
            jumpTrigger = true;
        }
        //if time is during 1 and 2, jump 1.5 times of original initialJumpVelocity
        else if (Input.GetKeyUp(KeyCode.Space) && chargingTime > 1f && chargingTime < 2f && isCharging)
        {
            isCharging = false;
            initialJumpVelocity *= 1.5f; //increase velocity to jump higher
            jumpTrigger = true;
        }
        //Max jump velocity, if holding longger than 2 seconds, jumps 1.8 times height
        else if (Input.GetKeyUp(KeyCode.Space) && chargingTime > 2f && isCharging)
        {
            isCharging = false;
            //initialJumpVelocity *= 2.5f;
            initialJumpVelocity *= 1.8f; //2.5 times of initialJumpVelocity is quite big, I wanna make it less.
            jumpTrigger = true;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        //Dash Trigger

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




        //Smash Trigger

        //if player is in the air and press "F"
        if(isGrounded == false && canSmash && Input.GetKeyDown(KeyCode.F))
        {
            smashTrigger = true; //doing downward smashing
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





    //Vertical and horizontal states

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }





    //Direction

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






    ///////////////////////////////////////COROUTINE////////////////////////////////////////////////////////////////////////////////////


    //Dash   &   Smash




    //Dashing coroutine -> fixedUpdate
    IEnumerator IsDashing()
    {
        isDashing = true; //play animation

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

        isDashing = false; //animation ends

        afterFirstDash = true; //after first dashing, I can set "canDash" to true for each time that player touches ground -> because I don't want player dash more than 1 time within the air
    }
    //Smashing Coroutine -> fixedUpdate as well.
    IEnumerator DownWardSmash()
    {
        isSmashing = true; //play animation

        //Only allow player smash once per jump.
        canSmash = false;

        //In case of charging, player remains the same position, so no gravity and y force.
        gravity = 0;
        currentVelocity.y = 0;
        playerRB.linearVelocityY = 0;

        float t = 0;
        while (t < 2f) //charging
        {
            t += Time.fixedDeltaTime;
            yield return null;
        }

        //give gravity back
        gravity = initialGravity;

        //Charging is finished, now smashing downwards
        while (!isGrounded) //keep smashing before player hits the ground
        {
            playerRB.linearVelocityY = smashSpeed;
            currentVelocity.y = playerRB.linearVelocityY;
            yield return null;
        }

        isSmashing = false;
        canSmash = true;
    }
}
