using System.Collections;
using UnityEngine;

public class BossTripleDashState : EnemyStateBase
{
    private ThirdBossController boss;
    private bool done;

    public BossTripleDashState(ThirdBossController boss, StateMachine sm)
        : base(boss, sm) => this.boss = boss;

    public override void Enter()
    {
        done = false;
        enemy.TriggerAnim("Bite");
        enemy.StartCoroutine(TripleDashRoutine());
    }

    public override void Update()
    {
        if (done) stateMachine.ChangeState(boss.GetRecoveryState());
    }

    private IEnumerator TripleDashRoutine()
    {
        boss.AllowRotation = false;
        for (int i = 0; i < 3; i++)
        {
            if (enemy.PlayerTransform == null) break;

            // Recible le joueur avant chaque dash
            Vector3 dashDir = (enemy.PlayerTransform.position - enemy.transform.position);
            dashDir.y = 0f;
            dashDir   = dashDir.normalized;

            enemy.transform.rotation = Quaternion.LookRotation(dashDir);

            // Petit windup
            yield return new WaitForSeconds(boss.TripleDashWindup);

            // Dash
            yield return enemy.StartCoroutine(
                SingleDash(dashDir, boss.TripleDashDistance, boss.TripleDashDuration));

            // Pause entre les dashes
            yield return new WaitForSeconds(boss.TripleDashPause);
        }
        boss.AllowRotation = true;
        yield return new WaitForSeconds(boss.TripleDashRecovery);
        done = true;
    }

    private IEnumerator SingleDash(Vector3 dir, float distance, float duration)
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos   = startPos + dir * distance;
        float   timer    = 0f;
        bool    hasHit   = false;

        int     hitboxMask = LayerMask.GetMask("PlayerHitbox");

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            enemy.Rb.MovePosition(Vector3.Lerp(startPos, endPos,
                                               Mathf.SmoothStep(0f, 1f, t)));

            // Un seul hit par dash
            if (!hasHit)
            {
                Collider[] hits = Physics.OverlapSphere(
                    enemy.transform.position, boss.TripleDashHitRadius, hitboxMask);

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hit.GetComponent<PlayerHealth>()
                           ?.TakeDamage(boss.TripleDashDamage);
                        hasHit = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        enemy.Rb.linearVelocity = Vector3.zero;
    }
}