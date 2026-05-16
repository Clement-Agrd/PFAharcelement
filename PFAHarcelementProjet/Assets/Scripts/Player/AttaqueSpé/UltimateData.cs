// Scripts/Ultimates/UltimateData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "UltimateData", menuName = "Roguelike/Ultimate Data")]
public class UltimateData : ScriptableObject
{
    [Header("Identité")]
    public string       ultimateName;
    public string       description;
    public Sprite       icon;
    public UltimateType type;

    [Header("Paramètres SpecialAttack")]
    public float specialDamage   = 30f;
    public float specialRadius   = 5f;
    public float specialCooldown = 8f;

    [Header("Paramètres StatBoost")]
    public float statBoostPercent  = 0.3f;  
    public float statBoostDuration = 10f;   

    [Header("Paramètres Shield")]
    public float shieldDuration = 5f;       
    public float shieldCooldown = 20f;      
}