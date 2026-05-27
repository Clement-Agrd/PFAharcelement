// Scripts/Ultimates/SonicShockwaveUltimate.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SonicShockwaveUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject shockwaveVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          shockwaveRadius;
    private float          shockwaveDuration;
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
        Debug.Log("🔓 Onde de choc débloquée");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float radius, float duration, float cooldown)
    {
        shockwaveRadius   = radius;
        shockwaveDuration = duration;
        baseCooldown      = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(ShockwaveCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator ShockwaveCoroutine()
    {
        isActive = true;

        // ← Son au début
        if (UltimateSoundManager.Instance != null)
            UltimateSoundManager.Instance.PlayShockwaveStart();

        if (shockwaveVFXPrefab != null)
        {
            GameObject vfx = Instantiate(shockwaveVFXPrefab,
                transform.position, Quaternion.identity);
            Destroy(vfx, shockwaveDuration + 1f);
        }

        Collider[]         hits           = Physics.OverlapSphere(transform.position, shockwaveRadius);
        List<EnemyStunned> stunnedEnemies = new List<EnemyStunned>();

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            EnemyStunned stunned = hit.gameObject.AddComponent<EnemyStunned>();
            stunned.Stun(shockwaveDuration);
            stunnedEnemies.Add(stunned);
        }

        Debug.Log($"💥 Onde de choc : {stunnedEnemies.Count} ennemis stunnés");

        yield return new WaitForSeconds(shockwaveDuration);

        isActive = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}