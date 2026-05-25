// Scripts/UI/BrightnessManager.cs
using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    [Header("Overlay")]
    public Image brightnessOverlay;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (brightnessOverlay != null)
            brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
    }

    void Start()
    {
        if (OptionsManager.Instance != null)
            ApplyBrightness(OptionsManager.Instance.GetData().brightness);
    }

    public void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null)
        {
            Debug.LogWarning("⚠️ BrightnessManager : overlay non assigné");
            return;
        }

        // value 1 = plein jour, value 0 = nuit noire
        float alpha = 1f - Mathf.Clamp01(value);
        brightnessOverlay.color = new Color(0f, 0f, 0f, alpha);

        Debug.Log($"🌙 Luminosité : {value} → alpha : {alpha}");
    }

    public void SetOverlay(Image overlay)
    {
        brightnessOverlay = overlay;

        if (brightnessOverlay != null)
            brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);

        if (OptionsManager.Instance != null)
            ApplyBrightness(OptionsManager.Instance.GetData().brightness);
    }
}