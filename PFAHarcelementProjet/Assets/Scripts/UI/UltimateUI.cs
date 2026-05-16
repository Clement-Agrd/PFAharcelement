using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateUI : MonoBehaviour
{
    [Header("Références")]
    public Image           ultimateIcon;
    public Image           cooldownFill;
    public TextMeshProUGUI cooldownText;
    public Button          ultimateButton;
    public Image           buttonImage;
    public GameObject      lockIcon;
    public GameObject      activeGlow; // effet visuel quand actif

    [Header("Couleurs")]
    public Color readyColor    = new Color(1f,   0.7f, 0f);
    public Color cooldownColor = new Color(0.3f, 0.3f, 0.3f);
    public Color activeColor   = new Color(1f,   1f,   0f);
    public Color lockedColor   = new Color(0.2f, 0.2f, 0.2f);

    private UltimateData currentData;

    void Start()
    {
        if (lockIcon != null)   lockIcon.SetActive(true);
        if (activeGlow != null) activeGlow.SetActive(false);
        if (ultimateButton != null)
            ultimateButton.onClick.AddListener(OnButtonClicked);
    }

    void Update()
    {
        if (currentData == null) return;

        UpdateCooldownUI();
    }

    public void SetUltimate(UltimateData data)
    {
        currentData = data;

        if (ultimateIcon != null && data.icon != null)
            ultimateIcon.sprite = data.icon;

        if (lockIcon != null)
            lockIcon.SetActive(false);

        if (buttonImage != null)
            buttonImage.color = readyColor;
    }

    public void OnActivate()
    {
        if (activeGlow != null)
            activeGlow.SetActive(true);

        if (buttonImage != null)
            buttonImage.color = activeColor;
    }

    void UpdateCooldownUI()
    {
        if (currentData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float remaining = 0f;
        float ratio     = 0f;
        bool  isActive  = false;
        bool  isReady   = false;

        switch (currentData.type)
        {
            case UltimateType.SpecialAttack:
                SpecialAttack sa = player.GetComponent<SpecialAttack>();
                if (sa != null)
                {
                    remaining = sa.GetCooldownRemaining();
                    ratio     = sa.GetCooldownRatio();
                    isReady   = sa.IsReady();
                }
                break;

            case UltimateType.StatBoost:
                StatBoostUltimate sb = player.GetComponent<StatBoostUltimate>();
                if (sb != null)
                {
                    remaining = sb.GetRemaining();
                    ratio     = sb.GetCooldownRatio();
                    isActive  = sb.IsActive;
                    isReady   = sb.IsReady();
                }
                break;

            case UltimateType.Shield:
                ShieldUltimate sh = player.GetComponent<ShieldUltimate>();
                if (sh != null)
                {
                    remaining = sh.GetRemaining();
                    ratio     = sh.GetCooldownRatio();
                    isActive  = sh.IsActive;
                    isReady   = sh.IsReady();
                }
                break;
        }

        if (cooldownFill != null)
            cooldownFill.fillAmount = isReady ? 0f : ratio;

        if (cooldownText != null)
            cooldownText.text = isReady ? "" : $"{remaining:F1}s";

        if (activeGlow != null)
            activeGlow.SetActive(isActive);

        if (buttonImage != null)
        {
            if (isActive)      buttonImage.color = activeColor;
            else if (isReady)  buttonImage.color = readyColor;
            else               buttonImage.color = cooldownColor;
        }

        if (ultimateButton != null)
            ultimateButton.interactable = isReady;
    }

    void OnButtonClicked()
    {
        if (currentData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        switch (currentData.type)
        {
            case UltimateType.SpecialAttack:
                player.GetComponent<SpecialAttack>()?.OnSpecialButtonPressed();
                break;

            case UltimateType.StatBoost:
                player.GetComponent<StatBoostUltimate>()?.Activate();
                break;

            case UltimateType.Shield:
                player.GetComponent<ShieldUltimate>()?.Activate();
                break;
        }
    }
}