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

    // ─── Retourne 3 ultimates aléatoires ─────────────────────────────────────

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

    // ─── Applique l'ultime choisi au joueur ───────────────────────────────────

    public void ApplyUltimate(UltimateData data)
    {
        currentUltimate = data;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        switch (data.type)
        {
            case UltimateType.SpecialAttack:
                ApplySpecialAttack(player, data);
                break;

            case UltimateType.StatBoost:
                ApplyStatBoost(player, data);
                break;

            case UltimateType.Shield:
                ApplyShield(player, data);
                break;
        }

        // Met à jour l'UI
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null)
            ui.SetUltimate(data);

        Debug.Log($"✅ Ultime appliqué : {data.ultimateName}");
    }

    void ApplySpecialAttack(GameObject player, UltimateData data)
    {
        SpecialAttack special = player.GetComponent<SpecialAttack>();
        if (special == null) return;

        special.baseDamage      = data.specialDamage;
        special.explosionRadius = data.specialRadius;
        special.baseCooldown    = data.specialCooldown;
        special.Unlock();
    }

    void ApplyStatBoost(GameObject player, UltimateData data)
    {
        StatBoostUltimate boost = player.GetComponent<StatBoostUltimate>();
        if (boost == null)
            boost = player.AddComponent<StatBoostUltimate>();

        boost.Setup(data.statBoostPercent, data.statBoostDuration);
    }

    void ApplyShield(GameObject player, UltimateData data)
    {
        ShieldUltimate shield = player.GetComponent<ShieldUltimate>();
        if (shield == null)
            shield = player.AddComponent<ShieldUltimate>();

        shield.Setup(data.shieldDuration, data.shieldCooldown);
    }
}