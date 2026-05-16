using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateCardUI : MonoBehaviour
{
    [Header("Références")]
    public Image           cardIcon;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public Button          chooseButton;

    private UltimateData      data;
    private UltimateChoiceUI  choiceUI;

    public void Setup(UltimateData ultimateData, UltimateChoiceUI parent)
    {
        data     = ultimateData;
        choiceUI = parent;

        if (cardIcon != null && data.icon != null)
            cardIcon.sprite = data.icon;

        if (cardName != null)
            cardName.text = data.ultimateName;

        if (cardDescription != null)
            cardDescription.text = data.description;

        chooseButton.onClick.RemoveAllListeners();
        chooseButton.onClick.AddListener(() => choiceUI.OnChoose(data));
    }
}