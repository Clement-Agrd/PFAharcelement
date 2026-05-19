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

    [Header("Paramètres SwarmOfPredators")]
    public int   swarmCount    = 5;
    public float swarmDamage   = 15f;
    public float swarmDuration = 8f;
    public float swarmCooldown = 20f;
    public GameObject swarmPrefab;

    [Header("Paramètres SonicShockwave")]
    public float shockwaveRadius   = 8f;
    public float shockwaveDuration = 3f;
    public float shockwaveCooldown = 15f;

    [Header("Paramètres Mirror")]
    public float mirrorDuration = 6f;
    public float mirrorCooldown = 18f;

    [Header("Paramètres Invisibility")]
    public float invisibilityDuration = 5f;
    public float invisibilityCooldown = 20f;

    [Header("Paramètres WeightOfSilence")]
    public float silenceRadius   = 10f;
    public float silenceSlowness = 0.8f;
    public float silenceDuration = 5f;
    public float silenceCooldown = 18f;

    [Header("Paramètres Regeneration")]
    public float regenPercent  = 0.3f;
    public float regenDuration = 8f;
    public float regenCooldown = 25f;
}