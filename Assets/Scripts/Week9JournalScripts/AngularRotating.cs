using UnityEngine;

public class AngularRotating : MonoBehaviour
{
    Rigidbody2D rigidbody2D; //reference

    void Start()
    {
        rigidbody2D = transform.GetComponent<Rigidbody2D>(); //reference is object that has this script (two sqaures)
        rigidbody2D.angularVelocity = 120f; //rotating speed is 120.
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //if press mouse's left button
        {
            rigidbody2D.angularDamping += 0.1f; //increase damping, angularVelocity slow down.
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get mouse position
        Vector2 closest = rigidbody2D.ClosestPoint(mousePos); //Smallest distance value between sqaure rigidbody and mouse position
        Debug.DrawLine(mousePos, closest, Color.black); //draw a line between them to see

        float distance = Vector2.Distance(mousePos, closest);
        Debug.Log("smallest distance to sqaure is" + distance); //print out distance value
    }
}
