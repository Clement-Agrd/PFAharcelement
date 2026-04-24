using UnityEngine;
using UnityEngine.UI;

public class ColorblindManager : MonoBehaviour
{
    public static ColorblindManager Instance { get; private set; }

    public Image overlayImage; // Image qui couvre tout l'écran

    // Couleurs des filtres
    private Color[] filters = {
        new Color(0, 0, 0, 0),           // Normal — transparent
        new Color(0.3f, 0.6f, 0, 0.15f), // Deutéranopie — filtre vert
        new Color(0.6f, 0.3f, 0, 0.15f)  // Protanopie — filtre rouge
    };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyColorblindMode(int index)
    {
        if (overlayImage == null)
        {
            Debug.LogWarning("⚠️ ColorblindManager : overlayImage non assignée");
            return;
        }

        if (index < 0 || index >= filters.Length) return;

        overlayImage.color = filters[index];
        Debug.Log($"✅ Mode daltonisme appliqué : index {index}");
    }
}