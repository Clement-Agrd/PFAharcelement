using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerBaseData baseData;

    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    // ─── API publique ─────────────────────────────────────────────────────────


    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        OnStatsChanged?.Invoke();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        OnStatsChanged?.Invoke();
    }

    public void ClearAllModifiers()
    {
        modifiers.Clear();
        OnStatsChanged?.Invoke();
    }
    

    public void ApplyBuff(BuffPickupData buff)
    {
        if (buff == null) return;

        foreach (StatModifierData data in buff.modifiers)
        {
            StatModifier modifier = new StatModifier(
                data.statType,
                data.modifierType,
                data.value
            );

            AddModifier(modifier);
        }

        OnStatsChanged?.Invoke();
    }

    public float GetStat(StatType stat)
    {
        float baseValue    = GetBaseValue(stat);
        float flatBonus    = 0f;
        float percentBonus = 0f;

        // Buffs du run (objets ramassés)
        foreach (StatModifier mod in modifiers)
        {
            if (mod.targetStat != stat) continue;
            if (mod.modifierType == ModifierType.Flat) flatBonus    += mod.value;
            else                                        percentBonus += mod.value;
        }

        // Bonus permanents de l'arbre de stats
        if (StatTreeManager.Instance != null)
        {
            StatNodeData node = StatTreeManager.Instance.treeData.nodes
                .Find(n => n.statType == stat);

            if (node != null)
            {
                float treeBonus = StatTreeManager.Instance.GetTotalBonus(stat);
                if (node.modifierType == ModifierType.Flat) flatBonus    += treeBonus;
                else                                         percentBonus += treeBonus;
            }
        }

        return (baseValue + flatBonus) * (1f + percentBonus);
    }

    // ─── Privé ────────────────────────────────────────────────────────────────

    private float GetBaseValue(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHealth:                return baseData.maxHealth;
            case StatType.Tankiness:         return baseData.tankiness;
            case StatType.MeleeDamage:       return baseData.meleeDamage;
            case StatType.RangedDamage:      return baseData.rangedDamage;
            case StatType.ProjectileSpeed:   return baseData.projectileSpeed;
            case StatType.AttackSpeed:       return baseData.attackSpeed;
            case StatType.LifeSteal:         return baseData.lifeSteal;
            case StatType.MoveSpeed:         return baseData.moveSpeed;
            case StatType.CooldownReduction: return baseData.cooldownReduction;
            default:
                Debug.LogWarning($"[PlayerStats] Stat inconnue : {stat}");
                return 0f;
        }
    }
    public float GetBaseStatValue(StatType stat)
    {
        return GetBaseValue(stat);
    }

    public IEnumerable<StatModifier> GetActiveModifiers()
    {
        return modifiers;
    }
    
    public System.Action OnStatsChanged;
}