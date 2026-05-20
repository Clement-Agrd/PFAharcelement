using System.Collections;
using UnityEngine;

public class BossLongDashState : EnemyStateBase
{
    private ThirdBossController boss;
    private bool done;

    // Détection dégâts
    private static readonly int playerHitboxLayer = -1; // initialisé au Enter
    private bool hasHit;

    public BossLongDashState(ThirdBossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        done   = false;
        hasHit = false;
        enemy.PlayAnim("Dash");
        enemy.StartCoroutine(LongDashRoutine());
    }

    public override void Update()
    {
        if (done) stateMachine.ChangeState(boss.GetRecoveryState());
    }

    private IEnumerator LongDashRoutine()
    {
        if (enemy.PlayerTransform == null) { done = true; yield break; }

        // Telegraph — regarde le joueur
        Vector3 dashDir = (enemy.PlayerTransform.position - enemy.transform.position);
        dashDir.y = 0f;
        dashDir   = dashDir.normalized;
        
        boss.AllowRotation = false;

        enemy.transform.rotation = Quaternion.LookRotation(dashDir);
        yield return new WaitForSeconds(boss.LongDashWindup);

        // Dash
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos   = startPos + dashDir * boss.LongDashDistance;
        float   timer    = 0f;
        float   interval = 0f; // timer pour les projectiles

        while (timer < boss.LongDashDuration)
        {
            timer    += Time.deltaTime;
            interval += Time.deltaTime;

            float t = timer / boss.LongDashDuration;
            enemy.Rb.MovePosition(Vector3.Lerp(startPos, endPos,
                                               Mathf.SmoothStep(0f, 1f, t)));

            // Dégâts de contact
            if (!hasHit)
            {
                int mask = LayerMask.GetMask("PlayerHitbox");
                Collider[] hits = Physics.OverlapSphere(
                    enemy.transform.position, boss.LongDashHitRadius, mask);

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hit.GetComponent<PlayerHealth>()?.TakeDamage(boss.LongDashDamage);
                        hasHit = true;
                        break;
                    }
                }
            }

            // Tire des projectiles sur les côtés à intervalle régulier
            if (interval >= boss.LongDashProjectileInterval)
            {
                interval = 0f;
                FireSideProjectiles(dashDir);
            }
            yield return null;
        }
        boss.AllowRotation = true;
        // Recovery
        enemy.Rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(boss.LongDashRecovery);
        done = true;
    }

    private void FireSideProjectiles(Vector3 dashDir)
    {
        // Perpendiculaire au dash = côtés gauche et droit
        Vector3 right = Vector3.Cross(Vector3.up, dashDir).normalized;
        Vector3 left  = -right;

        FireProjectile(right);
        FireProjectile(left);
    }

    private void FireProjectile(Vector3 dir)
    {
        if (boss.ProjectilePrefab == null) return;

        GameObject p = Object.Instantiate(
            boss.ProjectilePrefab,
            enemy.transform.position,
            Quaternion.LookRotation(dir)
        );
        p.GetComponent<ProjectileEnemy>()?.Init(dir);
    }
}