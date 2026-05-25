// Scripts/Player/SpecialAttack.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAttack : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject explosionVFX;
    public float      explosionRadius = 5f;
    public float      baseCooldown    = 8f;
    public float      baseDamage      = 30f;

    [Header("Visée")]
    public GameObject aimIndicator;
    public float      maxRange        = 15f;
    public float      gamepadAimSpeed = 25f;
    public float      gamepadDeadzone = 0.15f;

    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private PlayerStats    stats;
    private PlayerControls controls;
    private float          lastUseTime = -99f;
    private bool           isAiming    = false;
    private Vector3        aimPosition;

    void Awake()
    {
        stats    = GetComponent<PlayerStats>();
        controls = new PlayerControls();
        if (startUnlocked) Unlock();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.SpecialAttack.performed += OnSpecialInput;
    }

    void OnDisable()
    {
        controls.Player.SpecialAttack.performed -= OnSpecialInput;
        controls.Player.Disable();
    }

    public void Unlock()
    {
        IsUnlocked = true;
        Debug.Log("🔓 Attaque spéciale débloquée");
        SpecialAttackUI ui = FindObjectOfType<SpecialAttackUI>();
        if (ui != null) ui.OnUnlock();
    }

    // ─── Cooldown avec CooldownReduction ─────────────────────────────────────

    public float GetFinalCooldown()
    {
        float cdr = Mathf.Clamp01(stats.GetStat(StatType.CooldownReduction));
        return baseCooldown * (1f - cdr);
    }

    public float GetCooldownRemaining() => Mathf.Max(0f, GetFinalCooldown() - (Time.time - lastUseTime));
    public float GetCooldownRatio()     => GetCooldownRemaining() / GetFinalCooldown();
    public bool  IsReady()              => IsUnlocked && GetCooldownRemaining() <= 0f;

    void OnSpecialInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) { Debug.Log("🔒 Non débloqué"); return; }
        if (!isAiming) ToggleAim();
        else           Launch();
    }

    void Update()
    {
        if (!IsUnlocked) return;
        if (!isAiming)   return;

        Vector2 stick = Vector2.zero;
        if (Gamepad.current != null)
            stick = Gamepad.current.leftStick.ReadValue();

        if (stick.magnitude > gamepadDeadzone) HandleGamepadAim(stick);
        else                                   HandleMouseAim();

        UpdateIndicator();
    }

    void HandleMouseAim()
    {
        Ray   ray    = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float dist))
        {
            Vector3 point     = ray.GetPoint(dist);
            Vector3 direction = point - transform.position;
            if (direction.magnitude > maxRange)
                point = transform.position + direction.normalized * maxRange;
            aimPosition = point;
        }
    }

    void HandleGamepadAim(Vector2 stick)
    {
        Vector3 direction = new Vector3(stick.x, 0f, stick.y);
        Vector3 target    = transform.position + direction.normalized * maxRange;
        aimPosition       = Vector3.MoveTowards(aimPosition, target,
                            gamepadAimSpeed * Time.deltaTime);

        Vector3 offset = aimPosition - transform.position;
        if (offset.magnitude > maxRange)
            aimPosition = transform.position + offset.normalized * maxRange;
    }

    void UpdateIndicator()
    {
        if (aimIndicator == null) return;
        aimIndicator.transform.position = new Vector3(
            aimPosition.x,
            transform.position.y + 0.1f,
            aimPosition.z
        );
    }

    void ToggleAim()
    {
        if (!IsReady()) { Debug.Log($"⏳ Recharge : {GetCooldownRemaining():F1}s"); return; }
        isAiming    = true;
        aimPosition = transform.position + transform.forward * 3f;
        if (aimIndicator != null) aimIndicator.SetActive(true);
    }

    void CancelAim()
    {
        isAiming = false;
        if (aimIndicator != null) aimIndicator.SetActive(false);
    }

    public void Launch()
    {
        if (!IsReady()) return;
        CancelAim();
        StartCoroutine(ExplodeCoroutine());
        lastUseTime = Time.time;
    }

    public void OnSpecialButtonPressed()
    {
        if (!IsUnlocked) return;
        if (!isAiming) ToggleAim();
        else           Launch();
    }

    IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        Explode();
    }

    void Explode()
    {
        // Scale sur RangedDamage
        float damage = baseDamage + stats.GetStat(StatType.RangedDamage);

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, aimPosition, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        Collider[] hits = Physics.OverlapSphere(aimPosition, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            float distance = Vector3.Distance(aimPosition, hit.transform.position);
            float falloff  = 1f - Mathf.Clamp01(distance / explosionRadius);
            float finalDmg = damage * falloff;

            target.TakeDamage(finalDmg);

            float lifeSteal = stats.GetStat(StatType.LifeSteal);
            if (lifeSteal > 0f)
            {
                PlayerHealth ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.Heal(finalDmg * lifeSteal);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(isAiming ? aimPosition : transform.position, explosionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}