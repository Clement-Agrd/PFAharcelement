using UnityEngine;

public class BossBurstState : EnemyStateBase
{
    private BossController boss;
    private float          timer;
    private int            shotsFired;

    public BossBurstState(BossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        timer      = 0f;
        shotsFired = 0;
        enemy.PlayAnim("Attack");
    }

    public override void Update()
    {
        if (shotsFired >= boss.BurstCount)
        {
            stateMachine.ChangeState(boss.GetRecoveryState());
            return;
        }

        timer += Time.deltaTime;

        if (timer >= boss.BurstInterval)
        {
            timer = 0f;
            FireProjectile();
            shotsFired++;
        }
    }

    private void FireProjectile()
    {
        if (enemy.PlayerTransform == null || boss.FirePoint == null) return;

        Vector3 dir = (enemy.PlayerTransform.position - boss.FirePoint.position);
        dir.y = 0f;
        dir   = dir.normalized;

        // Légère dispersion pour rendre la rafale moins parfaite
        dir += new Vector3(
            Random.Range(-boss.BurstSpread, boss.BurstSpread),
            0f,
            Random.Range(-boss.BurstSpread, boss.BurstSpread)
        );

        GameObject p = Object.Instantiate(
            boss.ProjectilePrefab,
            boss.FirePoint.position,
            Quaternion.LookRotation(dir)
        );
        p.GetComponent<ProjectileEnemy>()?.Init(dir.normalized);
    }
}