using UnityEngine;

// ── IDLE ─────────────────────────────────────────────────────────────
public class IdleState : EnemyStateBase
{
    public IdleState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter() => enemy.PlayAnim("Idle");

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

    public override void Enter() => enemy.PlayAnim("Walk");

    public override void FixedUpdate()
    {
        if (enemy.PlayerTransform == null) return;

        float dist = enemy.DistanceToPlayer();

        // ❌ Trop loin → retour idle
        if (dist > enemy.chaseRange)
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        // ✅ Assez proche → stop déplacement et attaque
        if (dist <= enemy.stopChaseRange)
        {
            stateMachine.ChangeState(enemy.GetAttackState());
            return;
        }

        // 👉 Sinon → continuer à chase
        Vector3 dir = enemy.PlayerTransform.position - enemy.transform.position;
        dir.y = 0f;
        dir = dir.normalized;

        enemy.Rb.MovePosition(enemy.Rb.position + dir * enemy.moveSpeed * Time.fixedDeltaTime);

        Vector3 lookDir = enemy.PlayerTransform.position - enemy.transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10f
            );
    }
}

// ── ATTACK ───────────────────────────────────────────────────────────
public class AttackState : EnemyStateBase
{
    private float timer;

    public AttackState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter()
    {
        timer = 0f;
        enemy.PlayAnim("Attack");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        float dist = enemy.DistanceToPlayer();

        // ❌ Trop loin → revenir en chase
        if (dist > enemy.attackRange)
        {
            stateMachine.ChangeState(enemy.GetChaseState());
            return;
        }

        // ✅ Attaque en boucle
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
        enemy.PlayAnim("Hurt");
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
        enemy.PlayAnim("Death");
        enemy.Rb.linearVelocity = Vector3.zero;
        enemy.Rb.isKinematic    = true;
        enemy.Die();
    }
}