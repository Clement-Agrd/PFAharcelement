using UnityEngine;

public class VirtualCursorController : MonoBehaviour
{
    public static VirtualCursorController Instance { get; private set; }

    [Header("Références")]
    public VirtualCursor virtualCursor;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowCursor()
    {
        if (virtualCursor != null)
            virtualCursor.enabled = true;
    }

    public void HideCursor()
    {
        if (virtualCursor != null)
            virtualCursor.enabled = false;
    }
}