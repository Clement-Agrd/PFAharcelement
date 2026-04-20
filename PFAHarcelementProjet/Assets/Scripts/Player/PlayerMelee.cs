using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public int damage = 20;

    public Animator animator;

    float nextAttack;

    public void TryAttack()
    {
        if (Time.time < nextAttack) return;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange
        );

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hit.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            Attack(closestEnemy);
        }

        nextAttack = Time.time + attackCooldown;
    }

    void Attack(Transform enemy)
    {
        Vector3 direction = enemy.position - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.LookRotation(direction);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}