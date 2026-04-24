using UnityEngine;
using TMPro;

public class TextSizeManager : MonoBehaviour
{
    public static TextSizeManager Instance { get; private set; }

    // Tailles de base pour chaque mode
    private float[] sizes = { 14f, 18f, 22f }; // 0 petit, 1 normal, 2 grand

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyTextSize(int index)
    {
        if (index < 0 || index >= sizes.Length) return;

        float size = sizes[index];

        // Trouve tous les textes dans la scène
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in allTexts)
        {
            // On ne touche pas aux textes trop grands (titres)
            if (text.fontSize < 30f)
                text.fontSize = size;
        }

        Debug.Log($"✅ Taille texte appliquée : {size}px");
    }
}