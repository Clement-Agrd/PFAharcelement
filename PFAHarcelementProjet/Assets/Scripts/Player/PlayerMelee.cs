using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMelee : MonoBehaviour
{
    public bool IsAttacking { get; private set; }

    [Header("Détection")]
    public float hitRange  = 1.5f;
    public float coneAngle = 60f;

    [Header("Dash")]
    public float dashDistance = 3f;
    public float dashSpeed    = 12f;
    public float returnSpeed  = 10f;

    [Header("Timing")]
    public float pauseAtPeak = 0.3f;

    [Header("Références")]
    public Animator animator;

    private PlayerStats        stats;
    private PlayerController   controller;
    private PlayerInputHandler input;
    private CharacterController cc;

    private float nextAttack;

    void Awake()
    {
        stats      = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        input      = GetComponent<PlayerInputHandler>();
        cc         = GetComponent<CharacterController>();
    }

    // ─────────────────────────────────────────
    // API PUBLIQUE
    // ─────────────────────────────────────────

    public void TryAttack()
    {
        if (!controller.CanAct || IsAttacking)
            return;

        if (controller.IsDashing)
            return;

        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float cooldown = (1f / attackSpeed) * (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time < nextAttack)
            return;

        Vector3 direction = GetMouseAimDirection();
        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(direction);

        animator?.SetTrigger("Attack");
        StartCoroutine(MeleeRoutine(direction));

        nextAttack = Time.time + cooldown;
    }

    /// <summary>
    /// Appelé par RoomLoader ou autre système global
    /// </summary>
    public void CancelAttack()
    {
        StopAllCoroutines();
        IsAttacking = false;
    }

    // ─────────────────────────────────────────
    // CORE
    // ─────────────────────────────────────────

    IEnumerator MeleeRoutine(Vector3 direction)
    {
        IsAttacking = true;
        controller.SetActions(false);
        controller.SetMovement(false);

        bool wasMoving = input.MoveInput.magnitude > 0.1f;
        float traveled = 0f;

        // ▶ DASH AVANT
        while (traveled < dashDistance)
        {
            float step = dashSpeed * Time.deltaTime;
            cc.Move(direction * step);
            traveled += step;

            CheckConeDamage(direction);
            yield return null;
        }

        // ⏸ PAUSE AU PIC
        if (pauseAtPeak > 0f)
            yield return new WaitForSeconds(pauseAtPeak);

        // ◀ DASH RETOUR (SEULEMENT SI IMMOBILE)
        if (!wasMoving)
        {
            float returned = 0f;
            while (returned < dashDistance)
            {
                float step = returnSpeed * Time.deltaTime;
                cc.Move(-direction * step);
                returned += step;
                yield return null;
            }
        }

        controller.SetMovement(true);
        controller.SetActions(true);
        IsAttacking = false;
    }

    // ─────────────────────────────────────────
    // DAMAGE
    // ─────────────────────────────────────────

    void CheckConeDamage(Vector3 forward)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRange);

        foreach (Collider hit in hits)
        {
            // Ignore joueur
            if (hit.transform.root == transform.root)
                continue;

            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg == null)
                continue;

            Vector3 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(forward, toTarget);

            if (angle <= coneAngle * 0.5f)
            {
                ApplyDamage(dmg);
                break; // une seule cible
            }
        }
    }

    void ApplyDamage(IDamageable target)
    {
        float damage = stats.GetStat(StatType.MeleeDamage);
        target.TakeDamage(damage);

        float lifeSteal = stats.GetStat(StatType.LifeSteal);
        if (lifeSteal > 0f)
        {
            GetComponent<PlayerHealth>()?.Heal(damage * lifeSteal);
        }
    }

    // ─────────────────────────────────────────
    // AIM (SOURIS)
    // ─────────────────────────────────────────

    Vector3 GetMouseAimDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 dir = point - transform.position;
            dir.y = 0f;
            return dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.zero;
        }

        return Vector3.zero;
    }

#if UNITY_EDITOR
    // ─────────────────────────────────────────
    // DEBUG
    // ─────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);

        Vector3 left  = Quaternion.Euler(0, -coneAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0,  coneAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + left  * hitRange);
        Gizmos.DrawLine(transform.position, transform.position + right * hitRange);
    }
#endif
}
