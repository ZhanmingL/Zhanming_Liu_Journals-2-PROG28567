using UnityEngine;

public class MouseBoxCheck : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(3, 3); //The boxCast covers a 3x3 box range
    public LayerMask targetLayer; //Detect rigidbody in target layer
    bool check = false; //false when not found any rigidbodies

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //mousePos as the middle point of boxCast
        transform.position = mousePos;

        check = Physics2D.BoxCast(transform.position, boxSize, 0, Vector2.zero, 0, targetLayer);
        Color drawColor = Color.red;
        if (check)
        {
            drawColor = Color.green;
        }
        //Draw a square where boxCast is
        Debug.DrawLine(new Vector2(mousePos.x - boxSize.x / 2, mousePos.y + boxSize.y / 2), mousePos + boxSize / 2, drawColor);
        Debug.DrawLine(mousePos + boxSize / 2, new Vector2(mousePos.x + boxSize.x / 2, mousePos.y - boxSize.y / 2), drawColor);
        Debug.DrawLine(new Vector2(mousePos.x + boxSize.x / 2, mousePos.y - boxSize.y / 2), mousePos - boxSize / 2, drawColor);
        Debug.DrawLine(mousePos - boxSize / 2, new Vector2(mousePos.x - boxSize.x / 2, mousePos.y + boxSize.y / 2), drawColor);
    }
}
