using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected override void InitStates()
    {
        idleState   = new IdleState(this, StateMachine);
        chaseState  = new ChaseState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);
        hurtState   = new HurtState(this, StateMachine);   // 👈 manquant
        deathState  = new DeathState(this, StateMachine);  // 👈 manquant
    }

    // ✅ RangedEnemyController.cs
    public override void PerformAttack()
    {
        if (PlayerTransform == null) return;

        Vector3 dir = (PlayerTransform.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir) // oriente le sprite/mesh aussi
        );

        projectile.GetComponent<ProjectileEnemy>()?.Init(dir);
    }
}