// Scripts/Camera/CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;

    [Header("Position")]
    public Vector3 offset      = new Vector3(0, 15, -10);
    public float   smoothSpeed = 10f;

    [Header("Zoom")]
    public float minZoom = 10f;
    public float maxZoom = 25f;
    public float zoomSpeed = 3f;
    private float currentZoom = 15f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            currentZoom = cam.orthographicSize > 0
                ? cam.orthographicSize
                : offset.y;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("⚠️ CameraFollow : aucune cible assignée");
            return;
        }

        FollowTarget();
        HandleZoom();
    }

    void FollowTarget()
    {
        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y + offset.y,
            target.position.z + offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    void HandleZoom()
    {
        // 🖱️ Souris — molette
        float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            currentZoom -= scroll * zoomSpeed * 10f;

        // 🎮 Manette — gâchettes L2/R2 (axe 3 par défaut)
        float triggerZoom = UnityEngine.Input.GetAxis("Fire2");
        if (Mathf.Abs(triggerZoom) > 0.1f)
            currentZoom += triggerZoom * zoomSpeed * Time.deltaTime * 20f;

        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Caméra orthographique
        if (cam != null && cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, Time.deltaTime * zoomSpeed);
        // Caméra perspective
        else
            offset.y = Mathf.Lerp(offset.y, currentZoom, Time.deltaTime * zoomSpeed);
    }
}