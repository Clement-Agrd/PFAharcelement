using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatUIRow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI valueText;

    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    public Color normalColor   = Color.white;

    public void Set(
        Sprite sprite,
        float current,
        float preview,
        StatType type
    )
    {
        Debug.Log($"UI SET {type} : {current} -> {preview}");
        
        icon.sprite = sprite;

        if (Mathf.Approximately(current, preview))
        {
            valueText.text = FormatValue(current, type);
            valueText.color = normalColor;
            return;
        }

        string arrow = preview > current ? " → " : " → ";
        valueText.text =
            $"{FormatValue(current, type)}{arrow}{FormatValue(preview, type)}";

        valueText.color =
            preview > current ? positiveColor : negativeColor;
    }

    public void SetNormal(Sprite sprite, float value, StatType type)
    {
        icon.sprite = sprite;
        valueText.text = FormatValue(value, type);
        valueText.color = normalColor;
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