using UnityEngine;

public class BossRecoveryState : EnemyStateBase
{
    private BossController boss;
    private float          timer;

    public BossRecoveryState(BossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        timer = 0f;
        enemy.PlayAnim("Idle");
        // Regarde le joueur pendant la recovery
        LookAtPlayer();
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= boss.RecoveryDuration)
            boss.SelectNextPattern();
    }

    private void LookAtPlayer()
    {
        if (enemy.PlayerTransform == null) return;
        Vector3 dir = enemy.PlayerTransform.position - enemy.transform.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            enemy.transform.rotation = Quaternion.LookRotation(dir);
    }
}