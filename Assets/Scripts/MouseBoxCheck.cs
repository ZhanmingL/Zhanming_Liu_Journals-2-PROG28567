using UnityEngine;

public class MouseBoxCheck : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(3, 3);
    public LayerMask targetLayer;
    bool check = false;

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.zero, 0, targetLayer);
        check = hit.collider != null;

        
        
    }

    private void OnDrawGizmos()
    {
        if (check)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
