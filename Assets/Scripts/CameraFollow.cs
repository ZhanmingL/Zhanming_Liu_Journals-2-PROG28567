using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform targetHolder;
    public float followIntensity;

    public Tilemap tilemap;

    private Vector2 viewportHalfSize;
    private Camera followCamera;
    private float cameraZDepth;

    private float minCameraX, maxCameraX, minCameraY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        followCamera = GetComponent<Camera>();
        cameraZDepth = transform.position.z;
        CalculateCameraBounds();
    }

    void CalculateCameraBounds()
    {
        //Sets where the bounds are for the tilemap (based on where tiles have been placed in the scene)
        tilemap.CompressBounds();

        //This establishes the size of the camera's view space
        float orthoSize = followCamera.orthographicSize;

        //This takes into account the aspect ratio of the camera
        viewportHalfSize = new(orthoSize * followCamera.aspect, orthoSize);

        Vector3Int tilemapMin = tilemap.cellBounds.min;
        Vector3Int tilemapMax = tilemap.cellBounds.max;

        minCameraX = tilemapMin.x + viewportHalfSize.x + tilemap.transform.position.x;
        maxCameraX = tilemapMax.x - viewportHalfSize.x + tilemap.transform.position.x;
        minCameraY = tilemapMin.y + viewportHalfSize.y + tilemap.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = targetHolder.position;
        targetPosition.z = cameraZDepth;

        if (targetPosition.x < minCameraX)
        {
            targetPosition.x = minCameraX;
        }
        else if (targetPosition.x > maxCameraX)
        {
            targetPosition.x = maxCameraX;
        }

        if (targetPosition.y < minCameraY)
        {
            targetPosition.y = minCameraY;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, followIntensity * Time.deltaTime);

    }
}