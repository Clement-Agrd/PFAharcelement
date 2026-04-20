
using System;

[Serializable]
public class StatModifier
{
    public StatType   targetStat;
    public ModifierType modifierType;
    public float      value;

    public StatModifier(StatType stat, ModifierType type, float value)
    {
        this.targetStat   = stat;
        this.modifierType = type;
        this.value        = value;
    }
}

public enum ModifierType
{
    Flat,       // +10 HP, +2 Damage, etc.
    Percent     // +20% MoveSpeed, +15% AttackSpeed, etc.
}