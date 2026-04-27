using UnityEngine;

public class DashChaseState : EnemyStateBase
{
    private DashEnemyController dasher;
    private float               dashCooldownTimer;

    public DashChaseState(DashEnemyController enemy, StateMachine sm)
        : base(enemy, sm) => dasher = enemy;

    public override void Enter()
    {
        enemy.PlayAnim("Walk");
        dashCooldownTimer = dasher.DashCooldown; // on attend un cooldown avant le 1er dash
    }

    public override void Update()
    {
        dashCooldownTimer -= Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        if (enemy.PlayerTransform == null) return;

        float dist = enemy.DistanceToPlayer();

        // Hors de portée → idle
        if (dist > enemy.chaseRange)
        {
            stateMachine.ChangeState(enemy.GetIdleState());
            return;
        }

        // Cooldown écoulé + à portée → DASH
        if (dashCooldownTimer <= 0f && dist <= dasher.DashTriggerRange)
        {
            stateMachine.ChangeState(dasher.GetDashState());
            return;
        }

        // Approche jusqu'à DashTriggerRange
        if (dist > dasher.DashTriggerRange)
        {
            Vector3 dir = (enemy.PlayerTransform.position - enemy.transform.position);
            dir.y = 0f;
            dir   = dir.normalized;

            enemy.Rb.MovePosition(
                enemy.Rb.position + dir * enemy.moveSpeed * Time.fixedDeltaTime
            );

            if (dir != Vector3.zero)
                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * 10f
                );
        }
    }
}