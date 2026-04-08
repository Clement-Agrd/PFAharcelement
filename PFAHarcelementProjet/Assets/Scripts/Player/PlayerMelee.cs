using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Melee Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public int damage = 20;

    private PlayerInputHandler input;

    float nextAttack;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        Vector2 aim = input.AimInput;

        if(aim.magnitude > 0.5f)
        {
            TryAttack();
        }
    }
    void TryAttack()
    {
        if(Time.time < nextAttack) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange
        );

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach(Collider hit in hits)
        {
            if(hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hit.transform.position
                );

                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }
        if(closestEnemy != null)
        {
            Attack(closestEnemy);
            nextAttack = Time.time + attackCooldown;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void Attack(Transform enemy)
    {
        Vector3 direction = enemy.position - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.LookRotation(direction);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();

        if(health != null)
        {
            health.TakeDamage(damage);
        }
    }
}