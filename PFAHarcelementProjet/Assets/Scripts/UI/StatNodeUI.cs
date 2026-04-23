// Scripts/UI/StatNodeUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatNodeUI : MonoBehaviour
{
    [Header("Références UI")]
    public Image            icon;
    public TextMeshProUGUI  nameText;
    public TextMeshProUGUI  levelText;
    public TextMeshProUGUI  bonusText;
    public TextMeshProUGUI  costText;
    public Button           upgradeButton;
    public Image            fillBar;

    private StatNodeData nodeData;
    private StatTreeUI   treeUI;

    public void Setup(StatNodeData data, StatTreeUI parent)
    {
        nodeData = data;
        treeUI   = parent;

        if (icon != null && data.icon != null)
            icon.sprite = data.icon;

        nameText.text = data.displayName;

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => treeUI.OnUpgrade(nodeData));

        Refresh();
    }

    public void Refresh()
    {
        int   level    = StatTreeManager.Instance.GetLevel(nodeData.statType);
        bool  maxed    = level >= nodeData.maxLevel;
        bool  canBuy   = StatTreeManager.Instance.CanUpgrade(nodeData);
        float bonus    = StatTreeManager.Instance.GetTotalBonus(nodeData.statType);
        int   cost     = StatTreeManager.Instance.GetCostForNextLevel(nodeData);

        levelText.text = $"{level} / {nodeData.maxLevel}";

        string unit = nodeData.modifierType == ModifierType.Percent ? "%" : "";
        bonusText.text = bonus > 0 ? $"+{bonus}{unit}" : "—";

        costText.text          = maxed ? "MAX" : $"{cost} XP";
        upgradeButton.interactable = canBuy && !maxed;

        if (fillBar != null)
            fillBar.fillAmount = nodeData.maxLevel > 0
                ? (float)level / nodeData.maxLevel
                : 0f;
    }
}