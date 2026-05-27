// Scripts/Audio/UltimateSoundData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "UltimateSoundData",
    menuName = "Roguelike/Ultimate Sound Data")]
public class UltimateSoundData : ScriptableObject
{
    [Header("Meute de Requins")]
    public AudioClip swarmLaunch;

    [Header("Cri de Révolte (SpecialAttack)")]
    public AudioClip specialLaunch;

    [Header("Résilience (Regeneration)")]
    public AudioClip regenLoop;

    [Header("Disparaître (Invisibility)")]
    public AudioClip invisibilityStart;

    [Header("Retour de Flamme (Mirror)")]
    public AudioClip mirrorStart;
    public AudioClip mirrorReflect;

    [Header("Carapace Mentale (Shield)")]
    public AudioClip shieldStart;
    public AudioClip shieldHit;

    [Header("Colère Libérée (StatBoost)")]
    public AudioClip statBoostStart;

    [Header("Brise le Silence (Shockwave)")]
    public AudioClip shockwaveStart;
}