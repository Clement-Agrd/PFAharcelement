using System.Collections;
using UnityEngine;

public class OctoInkState : EnemyStateBase
{
    private OctoBossController octo;
    private bool done;
    

    public OctoInkState(OctoBossController boss, StateMachine sm)
        : base(boss, sm) => octo = boss;

    public override void Enter()
    {
        done = false;
        enemy.PlayAnim("Attack");
        enemy.StartCoroutine(InkRoutine());
    }

    public override void Update()
    {
        if (done) stateMachine.ChangeState(octo.GetRecoveryState());
    }

    private IEnumerator InkRoutine()
    {
        float halfCone = octo.InkConeAngle * 0.5f;

        float t = 0f;

        for (int i = 0; i < octo.InkProjectileCount; i++)
        {
            // Ping-pong entre -halfCone et +halfCone
            float angle = Mathf.Lerp(
                -halfCone,
                halfCone,
                Mathf.PingPong(t, 1f)
            );

            t += octo.InkSweepSpeed; // vitesse du balayage

            foreach (Transform fp in octo.InkFirePoints)
            {
                // Direction = forward tourné de "angle" degrés sur Y
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * enemy.transform.forward;

                GameObject p = Object.Instantiate(
                    octo.InkProjectilePrefab,
                    fp.position,
                    Quaternion.LookRotation(dir)
                );

                p.GetComponent<ProjectileEnemy>()?.Init(dir);
            }

            yield return new WaitForSeconds(octo.InkFireInterval);
        }

        yield return new WaitForSeconds(0.5f);
        done = true;
    }
}