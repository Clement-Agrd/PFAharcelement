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
    private float          shieldCooldown;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerHealth   playerHealth;
    private GameObject     shieldInstance;
    private ShieldVFX      shieldVFX;
    private ShieldCollider shieldCollider;

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, shieldCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => shieldCooldown > 0 ? GetRemaining() / shieldCooldown : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        controls     = new PlayerControls();
        playerHealth = GetComponent<PlayerHealth>();

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
        shieldCooldown = cooldown;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked)
        {
            Debug.Log("🔒 Bouclier non débloqué");
            return;
        }

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

        if (shieldVFXPrefab != null)
        {
            shieldInstance = Instantiate(
                shieldVFXPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            shieldInstance.transform.localPosition = shieldOffset;
            shieldVFX      = shieldInstance.GetComponent<ShieldVFX>();
            shieldCollider = shieldInstance.GetComponentInChildren<ShieldCollider>();

            // Active le collider du bouclier
            if (shieldCollider != null)
                shieldCollider.SetActive(true);
        }

        // Bloque les dégâts sur le joueur
        if (playerHealth != null)
            playerHealth.SetInvincible(true);

        Debug.Log($"🛡️ Bouclier actif {shieldDuration}s");

        yield return new WaitForSeconds(shieldDuration);

        // Désactive le collider
        if (shieldCollider != null)
            shieldCollider.SetActive(false);

        // Réactive les dégâts
        if (playerHealth != null)
            playerHealth.SetInvincible(false);

        if (shieldInstance != null)
            Destroy(shieldInstance);

        shieldCollider = null;
        shieldVFX      = null;
        isActive       = false;

        Debug.Log("🛡️ Bouclier terminé");
    }

    public void NotifyHit(Vector3 hitPoint)
    {
        if (shieldVFX != null)
            shieldVFX.OnHit(hitPoint);
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);

        Vector3 scale = new Vector3(4f, 2.5f, 6f);
        if (shieldVFXPrefab != null)
        {
            ShieldVFX vfx = shieldVFXPrefab.GetComponent<ShieldVFX>();
            if (vfx != null) scale = vfx.baseScale;
        }

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.TransformPoint(shieldOffset),
            transform.rotation,
            scale
        );
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = oldMatrix;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.TransformPoint(shieldOffset), 0.1f);
    }
}