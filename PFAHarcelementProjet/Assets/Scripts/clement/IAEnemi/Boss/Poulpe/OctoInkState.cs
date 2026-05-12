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
        float angle     = 0f;
        float angleStep = 360f / octo.InkProjectileCount;

        for (int i = 0; i < octo.InkProjectileCount; i++)
        {
            // Tire depuis les deux FirePoints en même temps
            foreach (Transform fp in octo.InkFirePoints)
            {
                // Direction radiale + légère variation verticale
                float   rad = angle * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(
                    Mathf.Cos(rad),
                    Random.Range(-0.1f, 0.2f),
                    Mathf.Sin(rad)
                ).normalized;

                GameObject p = Object.Instantiate(
                    octo.InkProjectilePrefab,
                    fp.position,
                    Quaternion.LookRotation(dir)
                );
                p.GetComponent<ProjectileEnemy>()?.Init(dir);
            }

            angle += angleStep;
            yield return new WaitForSeconds(octo.InkFireInterval);
        }

        // Attend que les projectiles aient quitté la zone
        yield return new WaitForSeconds(0.5f);
        done = true;
    }
}