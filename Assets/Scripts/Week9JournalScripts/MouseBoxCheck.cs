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
        Debug.DrawLine(new Vector2(mousePos.x - boxSize.x / 2, mousePos.y + boxSize.y / 2), mousePos + boxSize / 2, drawColor);
        Debug.DrawLine(mousePos + boxSize / 2, new Vector2(mousePos.x + boxSize.x / 2, mousePos.y - boxSize.y / 2), drawColor);
        Debug.DrawLine(new Vector2(mousePos.x + boxSize.x / 2, mousePos.y - boxSize.y / 2), mousePos - boxSize / 2, drawColor);
        Debug.DrawLine(mousePos - boxSize / 2, new Vector2(mousePos.x - boxSize.x / 2, mousePos.y + boxSize.y / 2), drawColor);
    }
}
