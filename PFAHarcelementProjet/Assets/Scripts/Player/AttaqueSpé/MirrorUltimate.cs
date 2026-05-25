// Scripts/Ultimates/MirrorUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorUltimate : MonoBehaviour
{
    [Header("Miroir")]
    public GameObject mirrorDiskPrefab;
    public float      diskOffset = 2f;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          mirrorDuration;
    private float          baseCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;
    private PlayerStats    stats;
    private GameObject     mirrorInstance;
    private MirrorDisk     mirrorDisk;

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
        Debug.Log("🔓 Miroir débloqué");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float duration, float cooldown)
    {
        mirrorDuration = duration;
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
        StartCoroutine(MirrorCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator MirrorCoroutine()
    {
        isActive = true;

        // ← Son au début
        if (UltimateSoundManager.Instance != null)
            UltimateSoundManager.Instance.PlayMirrorStart();

        if (mirrorDiskPrefab != null)
        {
            mirrorInstance = Instantiate(mirrorDiskPrefab,
                transform.position + transform.forward * diskOffset,
                transform.rotation);

            mirrorDisk = mirrorInstance.GetComponent<MirrorDisk>();
            if (mirrorDisk != null)
                mirrorDisk.Setup(stats, transform);
        }

        Debug.Log($"🪞 Miroir actif {mirrorDuration}s");

        yield return new WaitForSeconds(mirrorDuration);

        if (mirrorInstance != null)
            Destroy(mirrorInstance);

        mirrorDisk     = null;
        mirrorInstance = null;
        isActive       = false;
    }
}