using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public Vector3 offset = new Vector3(0, 12, -12);
    public float followSpeed = 5f;

    [Header("Edge Scrolling")]
    public float moveSpeed = 20f;
    public int edgeSize = 10;

    [Header("Zoom")]
    public float zoomSpeed = 10f;
    public float minZoom = 8f;
    public float maxZoom = 20f;

    private bool isLocked = true;

    void Update()
    {
        HandleLockToggle();
        HandleZoom();

        if (isLocked)
        {
            FollowTarget();
        }
        else
        {
            HandleEdgeScrolling();
        }
    }

    void HandleLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLocked = !isLocked;
        }

        // Re-lock rapide
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLocked = true;
        }
    }

    void FollowTarget()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    void HandleEdgeScrolling()
    {
        Vector3 move = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - edgeSize)
            move += transform.right;

        if (Input.mousePosition.x <= edgeSize)
            move -= transform.right;

        if (Input.mousePosition.y >= Screen.height - edgeSize)
            move += transform.forward;

        if (Input.mousePosition.y <= edgeSize)
            move -= transform.forward;

        move.y = 0;

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            offset.y -= scroll * zoomSpeed;
            offset.z += scroll * zoomSpeed;

            offset.y = Mathf.Clamp(offset.y, minZoom, maxZoom);
            offset.z = -offset.y;
        }
    }
}