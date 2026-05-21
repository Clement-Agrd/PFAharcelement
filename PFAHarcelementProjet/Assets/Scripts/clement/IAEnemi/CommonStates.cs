using UnityEngine;

// ── IDLE ─────────────────────────────────────────────────────────────
public class IdleState : EnemyStateBase
{
    public IdleState(EnemyController e, StateMachine sm) : base(e, sm) { }

    public override void Enter() => enemy.PlayAnim("Idle");

    public override void Update()
    {
        // Ne détecte pas le joueur invisible
        if (enemy.IsPlayerInvisible()) return;

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

        // Si le joueur devient invisible pendant la chase → retour idle
        if (enemy.IsPlayerInvisible())
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        float dist = enemy.DistanceToPlayer();

        if (dist > enemy.chaseRange)
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        if (dist <= enemy.stopChaseRange)
        {
            stateMachine.ChangeState(enemy.GetAttackState());
            return;
        }

        Vector3 dir = enemy.PlayerTransform.position - enemy.transform.position;
        dir.y = 0f;
        dir   = dir.normalized;

        enemy.Rb.MovePosition(enemy.Rb.position + dir * enemy.moveSpeed * Time.fixedDeltaTime);

        Vector3 lookDir = enemy.PlayerTransform.position - enemy.transform.position;
        lookDir.y = 0f;
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
        Debug.Log("aalalalalalalalalalalal");
        enemy.TriggerAnim("Attack"); // ✅ trigger
    }


    public override void Update()
    {
        timer += Time.deltaTime;

        // Si le joueur devient invisible → arrête d'attaquer
        if (enemy.IsPlayerInvisible())
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        float dist = enemy.DistanceToPlayer();

        if (dist > enemy.attackRange)
        {
            stateMachine.ChangeState(enemy.GetChaseState());
            return;
        }

        if (timer >= enemy.attackCooldown)
        {
            enemy.TriggerAnim("Attack"); // ✅ relancer l'anim ici
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
        enemy.TriggerAnim("Hurt");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer < duration) return;

        if (enemy.IsDead && enemy.GetDeathState() != null)
        {
            stateMachine.ChangeState(enemy.GetDeathState());
            return;
        }

        // Ne reprend pas le combat si le joueur est invisible
        if (!enemy.IsPlayerInvisible())
        {
            if (enemy.IsPlayerInRange(enemy.attackRange) && enemy.GetAttackState() != null)
            {
                stateMachine.ChangeState(enemy.GetAttackState());
                return;
            }

            if (enemy.IsPlayerInRange(enemy.chaseRange) && enemy.GetChaseState() != null)
            {
                stateMachine.ChangeState(enemy.GetChaseState());
                return;
            }
        }

        if (enemy.GetIdleState() != null)
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        Debug.LogError("❌ Aucun state valide après Hurt !");
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