// Scripts/UI/SpecialAttackUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialAttackUI : MonoBehaviour
{
    [Header("Références")]
    public SpecialAttack   specialAttack;
    public Image           cooldownFill;   // image en mode Filled pour le cooldown
    public TextMeshProUGUI cooldownText;   // texte du temps restant
    public Button          specialButton;  // bouton UI mobile/tablette
    public Image           buttonImage;    // image du bouton pour le griser

    [Header("Couleurs")]
    public Color readyColor    = new Color(0f,   0.8f, 1f);  // bleu eau
    public Color cooldownColor = new Color(0.3f, 0.3f, 0.3f); // gris
    public Color aimingColor   = new Color(1f,   0.8f, 0f);  // jaune visée

    void Awake()
    {
        if (specialButton != null)
            specialButton.onClick.AddListener(OnButtonClicked);
    }

    void Update()
    {
        if (specialAttack == null) return;

        bool  ready     = specialAttack.IsReady();
        float remaining = specialAttack.GetCooldownRemaining();
        float ratio     = specialAttack.GetCooldownRatio();

        // Barre de cooldown
        if (cooldownFill != null)
            cooldownFill.fillAmount = ready ? 0f : ratio;

        // Texte
        if (cooldownText != null)
            cooldownText.text = ready ? "" : $"{remaining:F1}s";

        // Couleur du bouton
        if (buttonImage != null)
            buttonImage.color = ready ? readyColor : cooldownColor;

        // Bouton interactable
        if (specialButton != null)
            specialButton.interactable = ready;
    }

    void OnButtonClicked()
    {
        if (specialAttack != null)
            specialAttack.OnSpecialButtonPressed();
    }
}