using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    public int damage = 10;

    protected override void InitStates()
    {
        idleState = new IdleState(this, StateMachine);
        chaseState = new ChaseState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);
    }

    public override void PerformAttack()
    {
        // collision / overlap / hitbox
        Debug.Log("Coup de mêlée infligé");
    }
}