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

    private float          regenPercent;
    private float          regenDuration;
    private float          regenCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;
    private PlayerStats    stats;

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, regenCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => regenCooldown > 0 ? GetRemaining() / regenCooldown : 0f;
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
        regenPercent  = percent;
        regenDuration = duration;
        regenCooldown = cooldown;
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

        GameObject regenVFXInstance = null;
        if (regenVFXPrefab != null)
        {
            regenVFXInstance = Instantiate(
                regenVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            regenVFXInstance.transform.localPosition = Vector3.zero;
        }

        float maxHP       = stats.GetStat(StatType.HP);
        float totalHeal   = maxHP * regenPercent;
        float healPerTick = totalHeal / regenDuration;
        float elapsed     = 0f;

        Debug.Log($"💚 Régénération : +{totalHeal:F0} HP sur {regenDuration}s");

        while (elapsed < regenDuration)
        {
            elapsed += Time.deltaTime;
            if (playerHealth != null)
                playerHealth.Heal(healPerTick * Time.deltaTime);
            yield return null;
        }

        if (regenVFXInstance != null)
            Destroy(regenVFXInstance);

        isActive = false;
        Debug.Log("💚 Régénération terminée");
    }
}