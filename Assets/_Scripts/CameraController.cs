using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] private float movementSpeed = 3f;

    [Header("Map Boundaries")]
    [SerializeField] private Transform mapContainer;

    private Camera mainCamera;
    private Vector2 dragStartPosition;
    private Vector2 cameraStartPosition;
    private bool isDragging = false;
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
        // Calculate map boundaries based on map container
        if (mapContainer != null)
        {
            // Calculate bounds from all child renderers
            CalculateMapBounds();
        }
        else
        {
            Debug.LogWarning("Map container not assigned. Camera won't be constrained to map boundaries.");
        }
    }

    private void CalculateMapBounds()
    {
        // Initialize with first child bounds
        Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            mapBounds = renderers[0].bounds;

            // Expand to include all other renderers
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
        // Handle mouse input for PC testing
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Input.mousePosition;
            cameraStartPosition = transform.position;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 dragDelta = (Vector2)Input.mousePosition - dragStartPosition;
            // Convert to world space units
            float dragDistance = dragDelta.magnitude / Screen.width * (mainCamera.orthographicSize * 2);

            // Move in the opposite direction of the drag
            Vector2 dragDirection = -dragDelta.normalized;
            Vector2 newPosition = cameraStartPosition + (dragDirection * dragDistance * movementSpeed);
            targetPosition = ConstrainToBounds(newPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Handle touch input for mobile
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
                // Convert to world space units based on screen size
                float dragDistance = dragDelta.magnitude / Screen.width * (mainCamera.orthographicSize * 2);

                // Move in opposite direction of drag
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
        // Smoothly move the camera to target position

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

        // Calculate camera half-size in world units
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Constrain position to keep camera within map bounds plus padding
        float minX = mapBounds.min.x + cameraWidth;
        float maxX = mapBounds.max.x - cameraWidth;
        float minY = mapBounds.min.y + cameraHeight;
        float maxY = mapBounds.max.y - cameraHeight;

        // If map is smaller than camera view, center the camera
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

    // Method to manually set map boundaries if needed
    public void SetBoundaries(Bounds bounds)
    {
        mapBounds = bounds;
    }

    /// <summary>
    /// Move the camera to a specific position when button is clicked
    /// </summary>
    /// <param name="position">Target world position to move camera to</param>
    public void MoveToPosition(Vector2 position)
    {
        targetPosition = ConstrainToBounds(position);
        isDragging = false;
    }

    /// <summary>
    /// Move the camera to a specific game object
    /// </summary>
    /// <param name="targetObject">Target game object to focus on</param>
    public void MoveToTarget(Transform targetObject)
    {
        if (targetObject != null)
        {
            MoveToPosition(targetObject.position);
        }
    }
}
