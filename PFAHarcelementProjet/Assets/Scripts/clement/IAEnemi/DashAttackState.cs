using UnityEngine;

public class DashAttackState : EnemyStateBase
{
    private DashEnemyController dasher;

    // ── Phases ───────────────────────────────────────────────────────
    private enum Phase { Windup, Dashing, Recovery }
    private Phase phase;

    private float   timer;
    private Vector3 dashDirection;
    private bool    hasHit; // on ne frappe qu'une fois par dash

    public DashAttackState(DashEnemyController enemy, StateMachine sm)
        : base(enemy, sm) => dasher = enemy;

    // ── Enter ─────────────────────────────────────────────────────────
    public override void Enter()
    {
        timer  = 0f;
        hasHit = false;
        phase  = Phase.Windup;

        dashDirection = (enemy.PlayerTransform.position - enemy.transform.position);
        dashDirection.y = 0f;
        dashDirection   = dashDirection.normalized;

        if (dashDirection != Vector3.zero)
            enemy.transform.rotation = Quaternion.LookRotation(dashDirection);

        enemy.Rb.linearVelocity = Vector3.zero;   // ← vide la vélocité accumulée
        enemy.Rb.isKinematic = true;               // ← plus pushable pendant le windup
        enemy.TriggerAnim("Speed");
    }

    // ── Update ────────────────────────────────────────────────────────
    public override void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            case Phase.Windup:    UpdateWindup();    break;
            case Phase.Dashing:   UpdateDashing();   break;
            case Phase.Recovery:  UpdateRecovery();  break;
        }
    }

    public override void FixedUpdate()
    {
        if (phase != Phase.Dashing) return;

        // Déplace via Rigidbody pour garder les collisions actives
        enemy.Rb.MovePosition(
            enemy.Rb.position + dashDirection * dasher.DashSpeed * Time.fixedDeltaTime
        );
    }

    // ── Phases logic ──────────────────────────────────────────────────
    private void UpdateWindup()
    {
        if (timer < dasher.WindupDuration) return;

        timer = 0f;
        phase = Phase.Dashing;
        enemy.TriggerAnim("Attack");

        enemy.Rb.isKinematic = false;              // ← reactivé pour MovePosition
        enemy.Rb.freezeRotation = true;
        enemy.Rb.constraints |= RigidbodyConstraints.FreezePositionY;
    }

    private void UpdateDashing()
    {
        if (!hasHit && enemy.IsPlayerInRange(dasher.DashHitRadius))
        {
            hasHit = true;
            HitPlayer();

            // ← Stoppe immédiatement le dash au contact
            timer = dasher.DashDuration;
            enemy.Rb.linearVelocity = Vector3.zero;
            return;
        }

        if (timer >= dasher.DashDuration)
        {
            timer = 0f;
            phase = Phase.Recovery;
            enemy.Rb.linearVelocity = Vector3.zero;
            enemy.PlayAnim("Idle");
        }
    }

    private void UpdateRecovery()
    {
        if (timer < dasher.RecoveryDuration) return;

        // Retour au cycle normal
        stateMachine.ChangeState(
            enemy.IsPlayerInRange(enemy.chaseRange)
                ? enemy.GetChaseState()
                : enemy.GetIdleState()
        );
    }

    // ── Hit ───────────────────────────────────────────────────────────
    private void HitPlayer()
    {
        enemy.PlayerTransform
             .GetComponent<PlayerHealth>()
             ?.TakeDamage(dasher.DashDamage);

        // Knockback sur le joueur
        Rigidbody playerRb = enemy.PlayerTransform.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockback = dashDirection * dasher.KnockbackForce + Vector3.up * 2f;
            playerRb.AddForce(knockback, ForceMode.Impulse);
        }
    }

    public override void Exit()
    {
        enemy.Rb.isKinematic = false;
        enemy.Rb.freezeRotation = false;
        enemy.Rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        enemy.Rb.linearVelocity = Vector3.zero;
    }
}