using UnityEngine;

public class CannonballController : MonoBehaviour
{
    public float lifespan;

    public GameObject leftCannon;
    public GameObject rightCannon;
    public GameObject cannonBall;
    
    void Start()
    {
        
        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(cannonBall, leftCannon.transform.position, Quaternion.identity);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Instantiate(cannonBall, rightCannon.transform.position, Quaternion.identity);
        }
    }
}
