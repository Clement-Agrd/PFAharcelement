// Scripts/Ultimates/ShieldUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShieldUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject shieldVFXPrefab;

    [Header("Position du bouclier")]
    public Vector3 shieldOffset = Vector3.zero;
    public bool    showGizmo    = true;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          shieldDuration;
    private float          baseCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;
    private PlayerStats    stats;
    private GameObject     shieldInstance;
    private ShieldVFX      shieldVFX;
    private ShieldCollider shieldCollider;

    public bool  IsActive           => isActive;
    public float GetFinalCooldown() => baseCooldown * (1f - Mathf.Clamp01(stats.GetStat(StatType.CooldownReduction)));
    public float GetRemaining()     => Mathf.Max(0f, GetFinalCooldown() - (Time.time - lastUseTime));
    public float GetCooldownRatio() => GetFinalCooldown() > 0 ? GetRemaining() / GetFinalCooldown() : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        controls     = new PlayerControls();
        playerHealth = GetComponent<PlayerHealth>();
        stats        = GetComponent<PlayerStats>();
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
        Debug.Log("🔓 Bouclier débloqué");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float duration, float cooldown)
    {
        shieldDuration = duration;
        baseCooldown   = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(ShieldCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator ShieldCoroutine()
    {
        isActive = true;

        // ← Son au début
        if (UltimateSoundManager.Instance != null)
            UltimateSoundManager.Instance.PlayShieldStart();

        if (shieldVFXPrefab != null)
        {
            shieldInstance = Instantiate(shieldVFXPrefab,
                transform.position, Quaternion.identity, transform);
            shieldInstance.transform.localPosition = shieldOffset;
            shieldVFX      = shieldInstance.GetComponent<ShieldVFX>();
            shieldCollider = shieldInstance.GetComponentInChildren<ShieldCollider>();
            if (shieldCollider != null) shieldCollider.SetActive(true);
        }

        if (playerHealth != null) playerHealth.SetInvincible(true);

        yield return new WaitForSeconds(shieldDuration);

        if (shieldCollider != null) shieldCollider.SetActive(false);
        if (playerHealth   != null) playerHealth.SetInvincible(false);
        if (shieldInstance != null) Destroy(shieldInstance);

        shieldCollider = null;
        shieldVFX      = null;
        isActive       = false;
    }

    public void NotifyHit(Vector3 hitPoint)
    {
        if (shieldVFX != null) shieldVFX.OnHit(hitPoint);
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Vector3 scale = new Vector3(4f, 2.5f, 6f);
        if (shieldVFXPrefab != null)
        {
            ShieldVFX vfx = shieldVFXPrefab.GetComponent<ShieldVFX>();
            if (vfx != null) scale = vfx.baseScale;
        }

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.TransformPoint(shieldOffset),
            transform.rotation, scale);
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = oldMatrix;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.TransformPoint(shieldOffset), 0.1f);
    }
}