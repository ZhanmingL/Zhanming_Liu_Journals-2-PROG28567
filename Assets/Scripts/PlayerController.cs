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

    public float velocity;

    public LayerMask checkGround;

    public float apexTime, apexHeight;

    public float gravity, initialJumpVelocity;

    private bool jumpTrigger;

    float xInput;


    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();


        //Set the gravity
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));

        //Set the initial jump velocity
        initialJumpVelocity = 2 * apexHeight / apexTime;
    }

    
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, checkGround);

        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;
        transform.eulerAngles = rotation;
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        //Vector2 playerInput = new Vector2();


        //check player input and use it as addForce's basic value
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticaltalInput = Input.GetAxisRaw("Vertical");
        Vector2 playerInput = new Vector2(horizontalInput, 0);

        MovementUpdate(playerInput);

        //I learned from totalForce reference, I use this code to check whether player is walking/jumping
        if(playerRB.totalForce.x != 0)
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

        if (jumpTrigger)
        {
            playerRB.linearVelocityY = initialJumpVelocity;
            jumpTrigger = false;
        }

        if(isGrounded == false)
        {
            playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;
        }
    }





    private void MovementUpdate(Vector2 playerInput)
    {
        //playerRB.AddForce(playerInput);
        xInput = playerRB.linearVelocityX;
        xInput = playerInput.x * velocity;
        playerRB.linearVelocityX = xInput;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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

        if (xInput < 0)
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
