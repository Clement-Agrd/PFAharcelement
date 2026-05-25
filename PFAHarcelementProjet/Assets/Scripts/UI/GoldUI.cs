// Scripts/UI/GoldUI.cs
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [Header("Références")]
    public TextMeshProUGUI goldText;
    public Animator        animator;

    void Start()
    {
        // S'abonne après que tout soit initialisé
        if (XPManager.Instance != null)
        {
            XPManager.Instance.OnGoldChanged += UpdateUI;
            UpdateUI(XPManager.Instance.GetGold());
        }
        else
            Debug.LogError("❌ GoldUI : XPManager.Instance null au Start");
    }

    void OnDisable()
    {
        if (XPManager.Instance != null)
            XPManager.Instance.OnGoldChanged -= UpdateUI;
    }

    void UpdateUI(int gold)
    {
        Debug.Log($"💰 GoldUI mise à jour : {gold}");

        if (goldText != null)
            goldText.text = $"{gold}";
        else
            Debug.LogError("❌ GoldUI : goldText non assigné");

        if (animator != null)
            animator.SetTrigger("Pulse");
    }
}