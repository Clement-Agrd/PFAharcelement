using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected override void InitStates()
    {
        idleState = new IdleState(this, StateMachine);
        chaseState = new ChaseState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);

        attackRange = 6f;
        attackCooldown = 1.6f;
    }

    public override void PerformAttack()
    {
        Vector2 dir =
            (PlayerTransform.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.GetComponent<Rigidbody2D>().linearVelocity = dir * 6f;
    }
}