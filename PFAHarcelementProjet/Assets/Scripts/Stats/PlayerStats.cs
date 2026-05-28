// PlayerStats.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerBaseData baseData;

    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    public System.Action OnStatsChanged;

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
            AddModifier(new StatModifier(data.statType, data.modifierType, data.value));
        }

        OnStatsChanged?.Invoke();
    }

    public float GetStat(StatType stat)
    {
        float baseValue    = GetBaseValue(stat);
        float flatBonus    = 0f;
        float percentBonus = 0f;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.targetStat != stat) continue;
            if (mod.modifierType == ModifierType.Flat) flatBonus    += mod.value;
            else                                        percentBonus += mod.value;
        }

        ApplyStatTree(stat, ref flatBonus, ref percentBonus);

        return Clamp(stat, (baseValue + flatBonus) * (1f + percentBonus));
    }

    // Preview avec un seul StatModifier (usage legacy)
    public float GetStatPreview(StatType stat, StatModifier previewModifier)
    {
        float baseValue    = GetBaseValue(stat);
        float flatBonus    = 0f;
        float percentBonus = 0f;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.targetStat != stat) continue;
            if (mod.modifierType == ModifierType.Flat) flatBonus    += mod.value;
            else                                        percentBonus += mod.value;
        }

        if (previewModifier != null && previewModifier.targetStat == stat)
        {
            if (previewModifier.modifierType == ModifierType.Flat) flatBonus    += previewModifier.value;
            else                                                     percentBonus += previewModifier.value;
        }

        ApplyStatTree(stat, ref flatBonus, ref percentBonus);

        return Clamp(stat, (baseValue + flatBonus) * (1f + percentBonus));
    }

    // Preview avec les modificateurs d'un buff (calcul correct depuis la base)
    public float GetStatPreviewWithBuff(StatType stat, IEnumerable<StatModifierData> buffMods)
    {
        float baseValue    = GetBaseValue(stat);
        float flatBonus    = 0f;
        float percentBonus = 0f;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.targetStat != stat) continue;
            if (mod.modifierType == ModifierType.Flat) flatBonus    += mod.value;
            else                                        percentBonus += mod.value;
        }

        foreach (StatModifierData data in buffMods)
        {
            if (data.statType != stat) continue;
            if (data.modifierType == ModifierType.Flat) flatBonus    += data.value;
            else                                         percentBonus += data.value;
        }

        ApplyStatTree(stat, ref flatBonus, ref percentBonus);

        return Clamp(stat, (baseValue + flatBonus) * (1f + percentBonus));
    }

    public float GetBaseStatValue(StatType stat) => GetBaseValue(stat);

    public IEnumerable<StatModifier> GetActiveModifiers() => modifiers;

    public void ResetRunStats()
    {
        modifiers.Clear();
        OnStatsChanged?.Invoke();
    }

    // ─── Privé ────────────────────────────────────────────────────────────────

    private float GetBaseValue(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHealth:         return baseData.maxHealth;
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

    // Factorisé : applique les bonus de l'arbre dans les deux accumulateurs
    private void ApplyStatTree(StatType stat, ref float flatBonus, ref float percentBonus)
    {
        if (StatTreeManager.Instance == null) return;

        StatNodeData node = StatTreeManager.Instance.treeData.nodes
            .Find(n => n.statType == stat);

        if (node == null) return;

        float treeBonus = StatTreeManager.Instance.GetTotalBonus(stat);

        if (node.modifierType == ModifierType.Flat) flatBonus    += treeBonus;
        else                                         percentBonus += treeBonus;
    }

    // Factorisé : clamp selon le type de stat
    private float Clamp(StatType stat, float value)
    {
        switch (stat)
        {
            case StatType.CooldownReduction:
                return Mathf.Clamp(value, 0f, 0.95f);
            default:
                return stat != StatType.MaxHealth ? Mathf.Max(0f, value) : value;
        }
    }
}