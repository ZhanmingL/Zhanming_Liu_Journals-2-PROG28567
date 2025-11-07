using UnityEngine;

public class AngularRotating : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = transform.GetComponent<Rigidbody2D>();
        rigidbody2D.angularVelocity = 120f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rigidbody2D.angularDamping += 0.1f;
        }
    }
}
