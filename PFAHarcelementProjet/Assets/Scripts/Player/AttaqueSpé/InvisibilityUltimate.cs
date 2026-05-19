// Scripts/Ultimates/InvisibilityUltimate.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InvisibilityUltimate : MonoBehaviour
{
    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private float          invisibilityDuration;
    private float          invisibilityCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;

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

        // Rend le joueur transparent
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            Color c = r.material.color;
            c.a = 0.2f;
            r.material.color = c;
        }

        // Désactive la détection par les ennemis
        gameObject.layer = LayerMask.NameToLayer("Invisible");

        Debug.Log($"👻 Invisibilité active {invisibilityDuration}s");

        yield return new WaitForSeconds(invisibilityDuration);

        // Remet le joueur visible
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            Color c = r.material.color;
            c.a = 1f;
            r.material.color = c;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");

        isActive = false;
        Debug.Log("👻 Invisibilité terminée");
    }
}