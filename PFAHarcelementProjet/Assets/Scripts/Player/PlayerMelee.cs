// Scripts/Player/PlayerMelee.cs
using System.Collections;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Détection")]
    public float detectionRange = 6f;  // grande zone de détection
    public float stopDistance   = 1f;  // distance à laquelle on arrête le dash

    [Header("Références")]
    public Animator animator;

    private PlayerStats      stats;
    private PlayerController controller;
    private float            nextAttack;

    void Awake()
    {
        stats      = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
    }

    public void TryAttack()
    {
        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float attackCooldown    = (1f / attackSpeed) * (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time < nextAttack) return;
        if (controller.IsDashing) return;

        Transform closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            Vector3 direction = closestEnemy.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");
            }

            StartCoroutine(DashToEnemy(closestEnemy));
        }

        nextAttack = Time.time + attackCooldown;
    }

    IEnumerator DashToEnemy(Transform enemy)
    {
        float elapsed     = 0f;
        float dashDuration = controller.dashDuration;
        float dashSpeed    = controller.dashSpeed;

        while (elapsed < dashDuration)
        {
            if (enemy == null) break;

            float dist = Vector3.Distance(transform.position, enemy.position);
            if (dist <= stopDistance) break;

            // Recalcule la direction à chaque frame pour suivre l'ennemi
            Vector3 direction = enemy.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            // Oriente le joueur en continu pendant le dash
            transform.rotation = Quaternion.LookRotation(direction);

            controller.StartDash(direction);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Applique les dégâts à la fin du dash
        Attack(enemy);
    }

    void Attack(Transform enemy)
    {
        if (enemy == null) return;

        float damage = stats.GetStat(StatType.MeleeDamage);

        IDamageable target = enemy.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);

            float lifeSteal = stats.GetStat(StatType.LifeSteal);
            if (lifeSteal > 0f)
            {
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                if (playerHealth != null)
                    playerHealth.Heal(damage * lifeSteal);
            }
        }
    }

    Transform FindClosestEnemy()
    {
        Transform closestEnemy = null;
        float     closestDist  = Mathf.Infinity;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist  = dist;
                closestEnemy = hit.transform;
            }
        }

        return closestEnemy;
    }

    void OnDrawGizmosSelected()
    {
        // Zone de détection en rouge
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Distance d'arrêt en jaune
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}