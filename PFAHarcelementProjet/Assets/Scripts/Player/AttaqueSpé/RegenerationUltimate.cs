// Scripts/Ultimates/RegenerationUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RegenerationUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject regenVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float           regenPercent;
    private float           regenDuration;
    private float           baseCooldown;
    private float           lastUseTime = -99f;
    private bool            isActive    = false;
    private PlayerControls  controls;
    private PlayerHealth    playerHealth;
    private PlayerStats     stats;
    private RegenerationVFX vfxInstance;

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
        Debug.Log("🔓 Régénération débloquée");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float percent, float duration, float cooldown)
    {
        regenPercent = percent;
        regenDuration = duration;
        baseCooldown  = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(RegenCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator RegenCoroutine()
    {
        isActive = true;

        if (regenVFXPrefab != null)
        {
            GameObject vfxGO = Instantiate(
                regenVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            vfxGO.transform.localPosition = Vector3.zero;
            vfxInstance = vfxGO.GetComponent<RegenerationVFX>();
        }

        // Scale sur HP max
        float maxHP     = stats.GetStat(StatType.MaxHealth);
        float totalHeal = maxHP * regenPercent;
        float elapsed   = 0f;

        Debug.Log($"💚 Régénération : +{totalHeal:F0} HP sur {regenDuration}s");

        while (elapsed < regenDuration)
        {
            elapsed += Time.deltaTime;
            if (playerHealth != null)
                playerHealth.Heal((totalHeal / regenDuration) * Time.deltaTime);
            yield return null;
        }

        if (vfxInstance != null)
            Destroy(vfxInstance.gameObject);

        isActive = false;
        Debug.Log("💚 Régénération terminée");
    }
}