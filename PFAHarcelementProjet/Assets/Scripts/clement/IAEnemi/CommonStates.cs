using UnityEngine;

// ── IDLE ─────────────────────────────────────────────────────────────
public class IdleState : EnemyStateBase
{
    public IdleState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter()  => enemy.Anim.Play("Idle");

    public override void Update()
    {
        if (enemy.IsPlayerInRange(enemy.chaseRange))
            stateMachine.ChangeState(enemy.GetChaseState());
    }
}

// ── CHASE ────────────────────────────────────────────────────────────
public class ChaseState : EnemyStateBase
{
    public ChaseState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter()  => enemy.Anim.Play("Walk");

    public override void FixedUpdate()
    {
        if (enemy.PlayerTransform == null) return;

        if (enemy.IsPlayerInRange(enemy.attackRange))
        {
            stateMachine.ChangeState(enemy.GetAttackState());
            return;
        }

        if (!enemy.IsPlayerInRange(enemy.chaseRange))
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        Vector2 dir = (enemy.PlayerTransform.position - enemy.transform.position).normalized;
        enemy.Rb.MovePosition(enemy.Rb.position + dir * enemy.moveSpeed * Time.fixedDeltaTime);

        // Flip sprite
        if (dir.x != 0)
            enemy.transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
    }
}

// ── ATTACK ───────────────────────────────────────────────────────────
public class AttackState : EnemyStateBase
{
    private float timer;

    public AttackState(EnemyController e, StateMachine sm)
        : base(e, sm) {}

    public override void Enter()
    {
        timer = 0f;
        enemy.Anim.Play("Attack");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (!enemy.IsPlayerInRange(enemy.attackRange))
        {
            stateMachine.ChangeState(enemy.GetChaseState());
            return;
        }

        if (timer >= enemy.attackCooldown)
        {
            enemy.PerformAttack();
            timer = 0f;
        }
    }
}

// ── HURT ─────────────────────────────────────────────────────────────
public class HurtState : EnemyStateBase
{
    private float duration;
    private float timer;

    public HurtState(EnemyController e, StateMachine sm, float duration = 0.35f)
        : base(e, sm) => this.duration = duration;

    public override void Enter()
    {
        timer = 0f;
        enemy.Anim.Play("Hurt");
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            stateMachine.ChangeState(enemy.GetChaseState());
    }
}

// ── DEATH ────────────────────────────────────────────────────────────
public class DeathState : EnemyStateBase
{
    public DeathState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter()
    {
        enemy.Anim.Play("Death");
        enemy.Rb.linearVelocity    = Vector2.zero;
        enemy.Rb.isKinematic = true;
        enemy.Die();
    }
}