// Scripts/UI/BrightnessOverlayScene.cs
using UnityEngine;
using UnityEngine.UI;

public class BrightnessOverlayScene : MonoBehaviour
{
    [Header("Références")]
    public Image brightnessOverlay;

    void Awake()
    {
        if (brightnessOverlay == null)
        {
            Debug.LogError("❌ BrightnessOverlayScene : overlay non assigné");
            return;
        }

        if (BrightnessManager.Instance != null)
        {
            BrightnessManager.Instance.SetOverlay(brightnessOverlay);
            Debug.Log("✅ BrightnessOverlayScene branché");
        }
        else
            Debug.LogError("❌ BrightnessManager.Instance introuvable");
    }
}