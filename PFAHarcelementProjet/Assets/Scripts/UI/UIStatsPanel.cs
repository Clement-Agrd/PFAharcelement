using UnityEngine;
using System.Collections.Generic;

public class UIStatsPanel : MonoBehaviour
{
    public PlayerStats playerStats;
    public StatUIRow rowPrefab;
    public Transform contentRoot;

    [System.Serializable]
    public class StatIcon
    {
        public StatType statType;
        public Sprite icon;
    }

    public List<StatIcon> statIcons;

    Dictionary<StatType, StatUIRow> rows = new();

    void Start()
    {
        foreach (var stat in statIcons)
        {
            StatUIRow row = Instantiate(rowPrefab, contentRoot);
            rows[stat.statType] = row;
            row.icon.sprite = stat.icon;
        }

        UpdateAll();

        playerStats.OnStatsChanged += UpdateAll;
    }

    void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateAll;
    }

    void UpdateAll()
    {
        foreach (var pair in rows)
        {
            float value = playerStats.GetStat(pair.Key);
            pair.Value.Set(
                GetSprite(pair.Key),
                value,
                pair.Key
            );
        }
    }

    Sprite GetSprite(StatType type)
    {
        return statIcons.Find(s => s.statType == type).icon;
    }
}
