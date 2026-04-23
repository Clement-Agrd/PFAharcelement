using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatTreeData", menuName = "Roguelike/Stat Tree Data")]
public class StatTreeData : ScriptableObject
{
    public List<StatNodeData> nodes = new List<StatNodeData>();
}

[System.Serializable]
public class StatNodeData
{
    [Header("Identité")]
    public string    displayName  = "Nom de la stat";
    public string    description  = "Description";
    public Sprite    icon;
    public StatType  statType;
    public ModifierType modifierType = ModifierType.Flat;

    [Header("Progression")]
    public int   maxLevel        = 5;
    public float valuePerLevel   = 10f;  // gain par rang
    public int   baseCost        = 100;  // coût du rang 1
    public float costMultiplier  = 2f;   // multiplicateur par rang
}