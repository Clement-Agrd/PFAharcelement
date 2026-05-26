using UnityEngine;
using System.Collections.Generic;

public class UIStatsPanel : MonoBehaviour
{
    public PlayerStats playerStats;
    public StatUIRow rowPrefab;
    public Transform contentRoot;
    
    

    public static UIStatsPanel Instance;

    void Awake()
    {
        Instance = this;
    }



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
    
    public void PreviewBuff(BuffPickupData buff)
    {
        foreach (var pair in rows)
        {
            StatType statType = pair.Key;

            float current = playerStats.GetStat(statType);

            float preview = current;

            // ✅ Applique TOUS les modifiers du buff en preview
            foreach (var mod in buff.modifiers)
            {
                if (mod.statType != statType) continue;

                StatModifier previewModifier = new StatModifier(
                    mod.statType,
                    mod.modifierType,
                    mod.value
                );

                preview = playerStats.GetStatPreview(statType, previewModifier);
            }

            pair.Value.Set(
                GetSprite(statType),
                current,
                preview,
                statType
            );
        }
    }
    
    public void ClearPreview()
    {
        UpdateAll();
    }



    void UpdateAll()
    {
        foreach (var pair in rows)
        {
            StatType stat = pair.Key;
            float value = playerStats.GetStat(stat);

            pair.Value.SetNormal(
                GetSprite(stat),
                value,
                stat
            );
        }
    }

    Sprite GetSprite(StatType type)
    {
        return statIcons.Find(s => s.statType == type).icon;
    }
}
