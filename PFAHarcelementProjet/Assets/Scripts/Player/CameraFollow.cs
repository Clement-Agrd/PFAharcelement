// Scripts/Camera/CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;

    [Header("Zoom initial")]
    public float startZoom = 6f;

    [Header("Position")]
    public Vector3 offset      = new Vector3(0, 4, -4);
    public float   smoothSpeed = 10f;

    [Header("Zoom")]
    public float minZoom = 4f;
    public float maxZoom = 14f;
    public float zoomSpeed = 3f;
    private float currentZoom = 6f;

    [Header("Offset Z dynamique")]
    public float minZOffset = -4f;
    public float maxZOffset = -12f;

    private Camera cam;


    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    
   
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (cam != null && cam.orthographic)
        {
            currentZoom = startZoom;
            cam.orthographicSize = startZoom; // ✅ PAS DE LERP
        }
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

        // 🎮 Manette — gâchettes
        float triggerZoom = UnityEngine.Input.GetAxis("Fire3");
        if (Mathf.Abs(triggerZoom) > 0.1f)
            currentZoom += triggerZoom * zoomSpeed * Time.deltaTime * 20f;

        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // -------- ZOOM CAMERA --------
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                currentZoom,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            offset.y = Mathf.Lerp(offset.y, currentZoom, Time.deltaTime * zoomSpeed);
        }

        // -------- OFFSET Z DYNAMIQUE --------
        float zoomT = Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
        float targetZ = Mathf.Lerp(minZOffset, maxZOffset, zoomT);

        offset.z = Mathf.Lerp(offset.z, targetZ, Time.deltaTime * zoomSpeed);
    }
}