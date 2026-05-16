// Scripts/Ultimates/StatBoostUltimate.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatBoostUltimate : MonoBehaviour
{
    private float          boostPercent;
    private float          boostDuration;
    private float          cooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerStats    stats;
    private PlayerControls controls;

    // Garde les modificateurs actifs pour pouvoir les retirer
    private List<StatModifier> activeModifiers = new List<StatModifier>();

    public bool  IsActive           => isActive;
    public float GetCooldown()      => cooldown;
    public float GetRemaining()     => Mathf.Max(0f, cooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => GetRemaining() / cooldown;
    public bool  IsReady()          => GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        stats    = GetComponent<PlayerStats>();
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.SpecialAttack.performed += OnInput;
    }

    void OnDisable()
    {
        controls.Player.SpecialAttack.performed -= OnInput;
        controls.Player.Disable();
    }

    public void Setup(float percent, float duration)
    {
        boostPercent  = percent;
        boostDuration = duration;
        cooldown      = duration * 3f; // cooldown = 3x la durée
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(BoostCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator BoostCoroutine()
    {
        isActive = true;

        // Applique le buff sur toutes les stats sauf HP
        activeModifiers.Clear();
        StatType[] statsToBoost = {
            StatType.MeleeDamage,
            StatType.RangedDamage,
            StatType.MoveSpeed,
            StatType.AttackSpeed,
            StatType.ProjectileSpeed,
            StatType.LifeSteal,
            StatType.CooldownReduction,
            StatType.Tankiness
        };

        foreach (StatType stat in statsToBoost)
        {
            StatModifier mod = new StatModifier(stat, ModifierType.Percent, boostPercent);
            activeModifiers.Add(mod);
            stats.AddModifier(mod);
        }

        Debug.Log($"⚡ StatBoost actif pendant {boostDuration}s");

        yield return new WaitForSeconds(boostDuration);

        // Retire les buffs
        foreach (StatModifier mod in activeModifiers)
            stats.RemoveModifier(mod);

        activeModifiers.Clear();
        isActive = false;

        Debug.Log("⚡ StatBoost terminé");
    }
}