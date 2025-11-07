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

        check = Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.zero, 0, targetLayer);
        Color drawColor = Color.red;
        if (check)
        {
            drawColor = Color.green;
        }
        Debug.DrawLine(new Vector2(transform.position.x + (transform.position.x - boxSize.x), boxSize.y), boxSize, drawColor);
        Debug.DrawLine(boxSize, new Vector2(transform.position.x + boxSize.x, transform.position.y + (transform.position.y - boxSize.y)));
        Debug.DrawLine(new Vector2(transform.position.x + boxSize.x, transform.position.y + (transform.position.y - boxSize.y)), new Vector2(transform.position.x - boxSize.x, transform.position.y - boxSize.y));
        Debug.DrawLine(new Vector2(transform.position.x + (transform.position.x - boxSize.x), transform.position.y + (transform.position.y - boxSize.y)), new Vector2(transform.position.x - boxSize.x, boxSize.y));
    }
}
