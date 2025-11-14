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

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.z = 0;
        transform.eulerAngles = rotation;
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        //Vector2 playerInput = new Vector2();


        //check player input and use it as addForce's basic value
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticaltalInput = Input.GetAxisRaw("Vertical");
        Vector2 playerInput = new Vector2(horizontalInput, verticaltalInput);

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
        if(playerRB.totalForce.y != 0)
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        playerRB.AddForce(playerInput);

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

        if (playerRB.totalForce.x < 0)
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
