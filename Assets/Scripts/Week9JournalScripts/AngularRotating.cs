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
    }
}
