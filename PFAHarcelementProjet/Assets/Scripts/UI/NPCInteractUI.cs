// Scripts/UI/NPCInteractUI.cs
using UnityEngine;
using TMPro;

public class NPCInteractUI : MonoBehaviour
{
    [Header("Références")]
    public GameObject      panel;
    public TextMeshProUGUI promptText;

    void Awake()
    {
        Debug.Log("✅ NPCInteractUI Awake");

        if (panel == null)
            Debug.LogError("❌ NPCInteractUI : panel NON assigné");
        if (promptText == null)
            Debug.LogError("❌ NPCInteractUI : promptText NON assigné");

        // Le GameObject parent reste actif
        // On cache juste le panel enfant
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(string key)
    {
        Debug.Log($"💬 NPCInteractUI.Show [{key}]");

        if (panel != null)
            panel.SetActive(true);

        if (promptText != null)
            promptText.text = $"[{key}] Parler";
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}