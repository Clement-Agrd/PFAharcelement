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
    private float            invisibilityCooldown;
    private float            lastUseTime = -99f;
    private bool             isActive    = false;
    private PlayerControls   controls;
    private InvisibilityVFX  vfxInstance;

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, invisibilityCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => invisibilityCooldown > 0 ? GetRemaining() / invisibilityCooldown : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
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
        Debug.Log("🔓 Invisibilité débloquée");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(float duration, float cooldown)
    {
        invisibilityDuration = duration;
        invisibilityCooldown = cooldown;
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

        // Instancie le VFX
        if (invisibilityVFXPrefab != null)
        {
            GameObject vfxGO = Instantiate(
                invisibilityVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            vfxGO.transform.localPosition = Vector3.zero;
            vfxInstance = vfxGO.GetComponent<InvisibilityVFX>();
        }

        // Fade out — devient invisible
        if (vfxInstance != null)
            vfxInstance.FadeOut();

        // Change le layer pour que les ennemis ignorent le joueur
        int originalLayer   = gameObject.layer;
        gameObject.layer    = LayerMask.NameToLayer("Invisible");

        Debug.Log($"👻 Invisibilité active {invisibilityDuration}s");

        yield return new WaitForSeconds(invisibilityDuration);

        // Remet le layer original
        gameObject.layer = originalLayer;

        // Fade in — redevient visible
        if (vfxInstance != null)
            vfxInstance.FadeIn();

        isActive = false;
        Debug.Log("👻 Invisibilité terminée");
    }
}