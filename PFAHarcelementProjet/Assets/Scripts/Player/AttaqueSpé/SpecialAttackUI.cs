// Scripts/UI/SpecialAttackUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialAttackUI : MonoBehaviour
{
    [Header("Références")]
    public SpecialAttack   specialAttack;
    public Image           cooldownFill;
    public TextMeshProUGUI cooldownText;
    public Button          specialButton;
    public Image           buttonImage;

    [Header("Icône verrouillé")]
    public GameObject lockIcon; // une image cadenas sur le bouton

    [Header("Couleurs")]
    public Color readyColor    = new Color(0f,   0.8f, 1f);
    public Color cooldownColor = new Color(0.3f, 0.3f, 0.3f);
    public Color lockedColor   = new Color(0.2f, 0.2f, 0.2f);

    void Awake()
    {
        if (specialButton != null)
            specialButton.onClick.AddListener(OnButtonClicked);
    }

    void Start()
    {
        // Cache le bouton si pas encore débloqué
        RefreshLockState();
    }

    void Update()
    {
        if (specialAttack == null) return;
        if (!specialAttack.IsUnlocked) return;

        bool  ready     = specialAttack.IsReady();
        float remaining = specialAttack.GetCooldownRemaining();
        float ratio     = specialAttack.GetCooldownRatio();

        if (cooldownFill != null)
            cooldownFill.fillAmount = ready ? 0f : ratio;

        if (cooldownText != null)
            cooldownText.text = ready ? "" : $"{remaining:F1}s";

        if (buttonImage != null)
            buttonImage.color = ready ? readyColor : cooldownColor;

        if (specialButton != null)
            specialButton.interactable = ready;
    }

    // Appelé par SpecialAttack.Unlock()
    public void OnUnlock()
    {
        RefreshLockState();
        Debug.Log("🔓 UI attaque spéciale débloquée");
    }

    void RefreshLockState()
    {
        if (specialAttack == null) return;

        bool unlocked = specialAttack.IsUnlocked;

        // Cache/montre l'icône cadenas
        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        // Grise le bouton si verrouillé
        if (buttonImage != null)
            buttonImage.color = unlocked ? readyColor : lockedColor;

        if (specialButton != null)
            specialButton.interactable = unlocked;

        if (cooldownText != null)
            cooldownText.text = unlocked ? "" : "🔒";
    }

    void OnButtonClicked()
    {
        if (specialAttack != null)
            specialAttack.OnSpecialButtonPressed();
    }
}