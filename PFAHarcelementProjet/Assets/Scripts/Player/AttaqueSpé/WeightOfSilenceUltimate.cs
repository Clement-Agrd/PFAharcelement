// Scripts/Ultimates/WeightOfSilenceUltimate.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeightOfSilenceUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject silenceVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          silenceRadius;
    private float          silenceSlowness;
    private float          silenceDuration;
    private float          baseCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerStats    stats;

    public bool  IsActive           => isActive;
    public float GetFinalCooldown() => baseCooldown * (1f - Mathf.Clamp01(stats.GetStat(StatType.CooldownReduction)));
    public float GetRemaining()     => Mathf.Max(0f, GetFinalCooldown() - (Time.time - lastUseTime));
    public float GetCooldownRatio() => GetFinalCooldown() > 0 ? GetRemaining() / GetFinalCooldown() : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        controls = new PlayerControls();
        stats    = GetComponent<PlayerStats>();
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
        Debug.Log("🔓 Poids du silence débloqué");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float radius, float slowness, float duration, float cooldown)
    {
        silenceRadius   = radius;
        silenceSlowness = slowness;
        silenceDuration = duration;
        baseCooldown    = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(SilenceCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator SilenceCoroutine()
    {
        isActive = true;

        if (silenceVFXPrefab != null)
        {
            GameObject vfx = Instantiate(silenceVFXPrefab,
                transform.position, Quaternion.identity);
            Destroy(vfx, silenceDuration + 2f);
        }

        Collider[]        hits          = Physics.OverlapSphere(transform.position, silenceRadius);
        List<EnemySlowed> slowedEnemies = new List<EnemySlowed>();

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            EnemySlowed slowed = hit.gameObject.AddComponent<EnemySlowed>();
            slowed.Slow(silenceSlowness, silenceDuration);
            slowedEnemies.Add(slowed);
        }

        Debug.Log($"🔇 Poids du silence : {slowedEnemies.Count} ennemis ralentis");

        yield return new WaitForSeconds(silenceDuration);

        isActive = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, silenceRadius);
    }
}