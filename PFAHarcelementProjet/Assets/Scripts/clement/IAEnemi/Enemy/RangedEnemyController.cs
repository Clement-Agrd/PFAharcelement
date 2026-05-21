using System.Collections;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float attackTiltAngle = 20f;
    public float tiltSpeed = 10f;

    private Coroutine tiltCoroutine;

    protected override void InitStates()
    {
        idleState   = new IdleState(this, StateMachine);
        chaseState  = new ChaseState(this, StateMachine);
        attackState = new AttackState(this, StateMachine);
        hurtState   = new HurtState(this, StateMachine);   // 👈 manquant
        deathState  = new DeathState(this, StateMachine);  // 👈 manquant
    }

    // ✅ RangedEnemyController.cs
    public override void PerformAttack()
    {
        if (PlayerTransform == null) return;

        // ✅ Lance le tilt
        if (tiltCoroutine != null)
            StopCoroutine(tiltCoroutine);

        tiltCoroutine = StartCoroutine(AttackTilt());
    }
    private IEnumerator AttackTilt()
    {
        Vector3 dir = (PlayerTransform.position - firePoint.position).normalized;
        float startAngle = transform.localEulerAngles.x;
        float targetAngle = startAngle + attackTiltAngle;

        float t = 0f;

        // 👉 Pencher
        while (t < 1f)
        {
            t += Time.deltaTime * tiltSpeed;

            float angle = Mathf.LerpAngle(startAngle, targetAngle, t);

            Vector3 euler = transform.localEulerAngles;
            euler.x = angle;
            transform.localEulerAngles = euler;

            yield return null;
        }
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        projectile.GetComponent<ProjectileEnemy>()?.Init(dir);

        t = 0f;

        // 👉 Revenir droit
        while (t < 1f)
        {
            t += Time.deltaTime * tiltSpeed;

            float angle = Mathf.LerpAngle(targetAngle, startAngle, t);

            Vector3 euler = transform.localEulerAngles;
            euler.x = angle;
            transform.localEulerAngles = euler;

            yield return null;
        }
    }
}