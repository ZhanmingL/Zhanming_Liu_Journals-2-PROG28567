using UnityEngine;

public class Plane : MonoBehaviour
{
    public Rigidbody2D planeRigibody;

    void Start()
    {
        //apply a force
        //planeRigibody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    void Update()
    {
        //planeRigibody.AddForce(-Vector3.up, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("This object has just collided with another.");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("This object has currently touching another.");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("This object has stopped colliding with another.");
    }
}
