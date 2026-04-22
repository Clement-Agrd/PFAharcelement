using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffPickup", menuName = "Roguelike/Buff Pickup")]
public class BuffPickupData : ScriptableObject
{
    [Header("Visuel")]
    public string buffName    = "Nouveau buff";
    public string description = "Description du buff";
    public Sprite icon;

    [Header("Modificateurs")]
    public List<StatModifierData> modifiers = new List<StatModifierData>();
}

[System.Serializable]
public class StatModifierData
{
    public StatType     statType;
    public ModifierType modifierType;
    public float        value;
}