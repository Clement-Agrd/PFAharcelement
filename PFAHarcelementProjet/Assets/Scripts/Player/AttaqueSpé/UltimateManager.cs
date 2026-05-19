// Scripts/Ultimates/UltimateManager.cs
using UnityEngine;
using System.Collections.Generic;

public class UltimateManager : MonoBehaviour
{
    public static UltimateManager Instance { get; private set; }

    [Header("Tous les ultimates disponibles")]
    public List<UltimateData> allUltimates = new List<UltimateData>();

    private UltimateData currentUltimate = null;

    public UltimateData CurrentUltimate => currentUltimate;
    public bool         HasUltimate     => currentUltimate != null;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Choix aléatoires ────────────────────────────────────────────────────

    public List<UltimateData> GetRandomChoices(int count = 3)
    {
        List<UltimateData> pool    = new List<UltimateData>(allUltimates);
        List<UltimateData> choices = new List<UltimateData>();

        count = Mathf.Min(count, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            choices.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return choices;
    }

    // ─── Application de l'ultime ─────────────────────────────────────────────

    public void ApplyUltimate(UltimateData data)
    {
        currentUltimate = data;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ UltimateManager : joueur introuvable");
            return;
        }

        switch (data.type)
        {
            case UltimateType.SpecialAttack:
                ApplySpecialAttack(player, data);   break;
            case UltimateType.StatBoost:
                ApplyStatBoost(player, data);       break;
            case UltimateType.Shield:
                ApplyShield(player, data);          break;
            case UltimateType.SwarmOfPredators:
                ApplySwarm(player, data);           break;
            case UltimateType.SonicShockwave:
                ApplyShockwave(player, data);       break;
            case UltimateType.Mirror:
                ApplyMirror(player, data);          break;
            case UltimateType.Invisibility:
                ApplyInvisibility(player, data);    break;
            case UltimateType.WeightOfSilence:
                ApplyWeightOfSilence(player, data); break;
            case UltimateType.Regeneration:
                ApplyRegeneration(player, data);    break;
        }

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null)
            ui.SetUltimate(data);

        Debug.Log($"✅ Ultime appliqué : {data.ultimateName}");
    }

    // ─── Méthodes Apply ──────────────────────────────────────────────────────

    void ApplySpecialAttack(GameObject player, UltimateData data)
    {
        SpecialAttack special = player.GetComponent<SpecialAttack>();
        if (special == null)
        {
            Debug.LogError("❌ SpecialAttack introuvable sur le joueur");
            return;
        }
        special.baseDamage      = data.specialDamage;
        special.explosionRadius = data.specialRadius;
        special.baseCooldown    = data.specialCooldown;
        special.Unlock();
    }

    void ApplyStatBoost(GameObject player, UltimateData data)
    {
        StatBoostUltimate boost = player.GetComponent<StatBoostUltimate>();
        if (boost == null) boost = player.AddComponent<StatBoostUltimate>();
        boost.Setup(data.statBoostPercent, data.statBoostDuration);
        boost.Unlock();
    }

    void ApplyShield(GameObject player, UltimateData data)
    {
        ShieldUltimate shield = player.GetComponent<ShieldUltimate>();
        if (shield == null) shield = player.AddComponent<ShieldUltimate>();
        shield.Setup(data.shieldDuration, data.shieldCooldown);
        shield.Unlock();
    }

    void ApplySwarm(GameObject player, UltimateData data)
    {
        SwarmOfPredatorsUltimate swarm = player.GetComponent<SwarmOfPredatorsUltimate>();
        if (swarm == null) swarm = player.AddComponent<SwarmOfPredatorsUltimate>();
        swarm.Setup(
            data.swarmCount,
            data.swarmDamage,
            data.swarmDuration,
            data.swarmCooldown,
            data.swarmPrefab
        );
        swarm.Unlock();
    }

    void ApplyShockwave(GameObject player, UltimateData data)
    {
        SonicShockwaveUltimate shockwave = player.GetComponent<SonicShockwaveUltimate>();
        if (shockwave == null) shockwave = player.AddComponent<SonicShockwaveUltimate>();
        shockwave.Setup(
            data.shockwaveRadius,
            data.shockwaveDuration,
            data.shockwaveCooldown
        );
        shockwave.Unlock();
    }

    void ApplyMirror(GameObject player, UltimateData data)
    {
        MirrorUltimate mirror = player.GetComponent<MirrorUltimate>();
        if (mirror == null) mirror = player.AddComponent<MirrorUltimate>();
        mirror.Setup(data.mirrorDuration, data.mirrorCooldown);
        mirror.Unlock();
    }

    void ApplyInvisibility(GameObject player, UltimateData data)
    {
        InvisibilityUltimate invis = player.GetComponent<InvisibilityUltimate>();
        if (invis == null) invis = player.AddComponent<InvisibilityUltimate>();
        invis.Setup(data.invisibilityDuration, data.invisibilityCooldown);
        invis.Unlock();
    }

    void ApplyWeightOfSilence(GameObject player, UltimateData data)
    {
        WeightOfSilenceUltimate silence = player.GetComponent<WeightOfSilenceUltimate>();
        if (silence == null) silence = player.AddComponent<WeightOfSilenceUltimate>();
        silence.Setup(
            data.silenceRadius,
            data.silenceSlowness,
            data.silenceDuration,
            data.silenceCooldown
        );
        silence.Unlock();
    }

    void ApplyRegeneration(GameObject player, UltimateData data)
    {
        RegenerationUltimate regen = player.GetComponent<RegenerationUltimate>();
        if (regen == null) regen = player.AddComponent<RegenerationUltimate>();
        regen.Setup(data.regenPercent, data.regenDuration, data.regenCooldown);
        regen.Unlock();
    }
}