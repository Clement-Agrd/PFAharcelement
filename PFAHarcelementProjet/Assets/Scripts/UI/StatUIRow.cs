using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatUIRow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI valueText;

    public void Set(Sprite sprite, float value, StatType type)
    {
        icon.sprite = sprite;

        valueText.text = FormatValue(value, type);
    }

    string FormatValue(float value, StatType type)
    {
        switch (type)
        {
            case StatType.LifeSteal:
            case StatType.CooldownReduction:
                return (value * 100f).ToString("0") + "%";

            case StatType.AttackSpeed:
            case StatType.MoveSpeed:
                return value.ToString("0.00");

            default:
                return value.ToString("0.0");
        }
    }
}