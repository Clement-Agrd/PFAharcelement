using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float range = 2f;
    public int damage = 10;
    public LayerMask enemyLayer;

    public Transform attackPoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, range, enemyLayer);

        foreach (Collider enemy in hits)
        {
            Debug.Log("Hit " + enemy.name);

            // Exemple futur :
            // enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}