// Scripts/Ultimates/StatBoostUltimate.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatBoostUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject statBoostVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          boostPercent;
    private float          boostDuration;
    private float          cooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerStats    stats;
    private PlayerControls controls;
    private StatBoostVFX   vfxInstance;

    private List<StatModifier> activeModifiers = new List<StatModifier>();

    public bool  IsActive           => isActive;
    public float GetCooldown()      => cooldown;
    public float GetRemaining()     => Mathf.Max(0f, cooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => cooldown > 0 ? GetRemaining() / cooldown : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        stats    = GetComponent<PlayerStats>();
        controls = new PlayerControls();

        if (startUnlocked) Unlock();
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

    public void Unlock()
    {
        IsUnlocked = true;
        Debug.Log("🔓 StatBoost débloqué");

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float percent, float duration)
    {
        boostPercent  = percent;
        boostDuration = duration;
        cooldown      = duration * 3f;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked)
        {
            Debug.Log("🔒 StatBoost non débloqué");
            return;
        }

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

        if (statBoostVFXPrefab != null)
        {
            GameObject vfxGO = Instantiate(
                statBoostVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            vfxGO.transform.localPosition = Vector3.zero;
            vfxInstance = vfxGO.GetComponent<StatBoostVFX>();
            if (vfxInstance != null) vfxInstance.Activate();
        }

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

        Debug.Log($"⚡ StatBoost actif {boostDuration}s");

        yield return new WaitForSeconds(boostDuration);

        foreach (StatModifier mod in activeModifiers)
            stats.RemoveModifier(mod);

        activeModifiers.Clear();

        if (vfxInstance != null)
            vfxInstance.Deactivate();

        isActive = false;
        Debug.Log("⚡ StatBoost terminé");
    }
}