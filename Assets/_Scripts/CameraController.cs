using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private Transform mapContainer;

    private bool isDragging = false;
    private Camera mainCamera;
    private Vector2 dragStartPosition;
    private Vector2 cameraStartPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector2 targetPosition;
    private Bounds mapBounds;
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        targetPosition = transform.position;
    }

    private void Start()
    {
        if (mapContainer) CalculateMapBounds();
    }

    private void CalculateMapBounds()
    {
        Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            mapBounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                mapBounds.Encapsulate(renderer.bounds);
            }
        }
    }

    private void Update()
    {
        HandleInput();
        MoveCamera();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragStartPosition = touch.position;
                cameraStartPosition = transform.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 dragDelta = touch.position - dragStartPosition;
                float dragDistance = dragDelta.magnitude / Screen.width * (mainCamera.orthographicSize * 2);
                Vector2 dragDirection = -dragDelta.normalized;
                Vector2 newPosition = cameraStartPosition + (dragDirection * dragDistance * movementSpeed);
                targetPosition = ConstrainToBounds(newPosition);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    private void MoveCamera()
    {
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.position,
            new Vector3(targetPosition.x, targetPosition.y, transform.position.z),
            ref velocity,
            0
        );
        transform.position = newPosition;
    }

    private Vector2 ConstrainToBounds(Vector2 position)
    {
        if (mapContainer == null)
            return position;

        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float minX = mapBounds.min.x + cameraWidth;
        float maxX = mapBounds.max.x - cameraWidth;
        float minY = mapBounds.min.y + cameraHeight;
        float maxY = mapBounds.max.y - cameraHeight;

        if (maxX < minX)
        {
            float centerX = (mapBounds.min.x + mapBounds.max.x) / 2f;
            position.x = centerX;
        }
        else
        {
            position.x = Mathf.Clamp(position.x, minX, maxX);
        }

        if (maxY < minY)
        {
            float centerY = (mapBounds.min.y + mapBounds.max.y) / 2f;
            position.y = centerY;
        }
        else
        {
            position.y = Mathf.Clamp(position.y, minY, maxY);
        }

        return position;
    }
}
