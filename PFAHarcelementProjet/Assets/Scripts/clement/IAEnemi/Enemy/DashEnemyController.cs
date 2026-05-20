using UnityEngine;

public class DashEnemyController : EnemyController
{
    [Header("Dash — Distances")]
    public float DashTriggerRange = 6f;   // distance à laquelle il déclenche le dash
    public float DashHitRadius    = 1.2f; // rayon de contact pour infliger les dégâts

    [Header("Dash — Timings")]
    public float WindupDuration   = 0.5f; // temps de charge avant le dash
    public float DashDuration     = 0.3f; // durée du dash lui-même
    public float RecoveryDuration = 0.8f; // lag après le dash
    public float DashCooldown     = 2.5f; // cooldown avant de pouvoir re-dasher

    [Header("Dash — Forces")]
    public float DashSpeed        = 18f;
    public float DashDamage       = 20f;
    public float KnockbackForce   = 8f;

    // État spécifique au dash
    private EnemyStateBase dashState;
    public  EnemyStateBase GetDashState() => dashState;

    protected override void InitStates()
    {
        idleState   = new IdleState(this, StateMachine);
        chaseState  = new DashChaseState(this, StateMachine); // chase custom
        attackState = dashState = new DashAttackState(this, StateMachine);
        hurtState   = new HurtState(this, StateMachine, 0.2f);
        deathState  = new DeathState(this, StateMachine);

        // Le dash se déclenche à DashTriggerRange, pas à attackRange
        attackRange = DashTriggerRange;
        chaseRange  = 12f;
    }
    
    public override void PerformAttack() { } // géré directement par DashAttackState

    public override void Die()
    {
        Debug.Log("Le Dasher fou est mort !");
        base.Die();
    }
}