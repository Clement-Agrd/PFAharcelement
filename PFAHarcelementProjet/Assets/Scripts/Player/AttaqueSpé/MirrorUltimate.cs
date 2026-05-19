// Scripts/Ultimates/MirrorUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorUltimate : MonoBehaviour
{
    [Header("VFX")]
    public GameObject mirrorVFXPrefab;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          mirrorDuration;
    private float          mirrorCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;
    private PlayerStats    stats;
    private GameObject     mirrorVFXInstance;

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, mirrorCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => mirrorCooldown > 0 ? GetRemaining() / mirrorCooldown : 0f;
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
        mirrorCooldown = cooldown;
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

        if (mirrorVFXPrefab != null)
        {
            mirrorVFXInstance = Instantiate(
                mirrorVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            mirrorVFXInstance.transform.localPosition = Vector3.zero;
        }

        // Active le renvoi de dégâts
        if (playerHealth != null)
            playerHealth.SetMirror(true);

        Debug.Log($"🪞 Miroir actif {mirrorDuration}s");

        yield return new WaitForSeconds(mirrorDuration);

        if (playerHealth != null)
            playerHealth.SetMirror(false);

        if (mirrorVFXInstance != null)
            Destroy(mirrorVFXInstance);

        isActive = false;
        Debug.Log("🪞 Miroir terminé");
    }
}