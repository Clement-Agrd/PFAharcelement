// Scripts/Ultimates/InvisibilityUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InvisibilityUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject invisibilityVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float            invisibilityDuration;
    private float            baseCooldown;
    private float            lastUseTime = -99f;
    private bool             isActive    = false;
    private PlayerControls   controls;
    private PlayerStats      stats;
    private InvisibilityVFX  vfxInstance;

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
        Debug.Log("🔓 Invisibilité débloquée");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float duration, float cooldown)
    {
        invisibilityDuration = duration;
        baseCooldown         = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(InvisibilityCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator InvisibilityCoroutine()
    {
        isActive = true;

        // ← Son au début
        if (UltimateSoundManager.Instance != null)
            UltimateSoundManager.Instance.PlayInvisibilityStart();

        if (invisibilityVFXPrefab != null)
        {
            GameObject vfxGO = Instantiate(invisibilityVFXPrefab,
                transform.position, Quaternion.identity, transform);
            vfxGO.transform.localPosition = Vector3.zero;
            vfxInstance = vfxGO.GetComponent<InvisibilityVFX>();
        }

        if (vfxInstance != null) vfxInstance.FadeOut();

        int originalLayer = gameObject.layer;
        gameObject.layer  = LayerMask.NameToLayer("Invisible");

        Debug.Log($"👻 Invisibilité active {invisibilityDuration}s");

        yield return new WaitForSeconds(invisibilityDuration);

        gameObject.layer = originalLayer;
        if (vfxInstance != null) vfxInstance.FadeIn();

        isActive = false;
    }
}