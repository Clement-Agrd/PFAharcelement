using UnityEngine;

public class ThirdBossController : EnemyController
{
    // ── Summon ────────────────────────────────────────────────────────
    [Header("Summon Pattern")]
    public GameObject SummonPrefab;
    public int        SummonCount  = 4;
    public float      SummonRadius = 4f;
    public float      SummonWindup = 1f;

    // ── Long Dash ─────────────────────────────────────────────────────
    [Header("Long Dash Pattern")]
    public GameObject ProjectilePrefab;
    public float      LongDashWindup            = 0.6f;
    public float      LongDashDistance          = 18f;
    public float      LongDashDuration          = 0.4f;
    public float      LongDashDamage            = 30f;
    public float      LongDashHitRadius         = 1.5f;
    public float      LongDashProjectileInterval = 0.08f; // intervalle entre chaque salve
    public float      LongDashRecovery          = 0.8f;

    // ── Triple Dash ───────────────────────────────────────────────────
    [Header("Triple Dash Pattern")]
    public float TripleDashWindup   = 0.25f;
    public float TripleDashDistance = 6f;
    public float TripleDashDuration = 0.2f;
    public float TripleDashDamage   = 20f;
    public float TripleDashHitRadius = 1.2f;
    public float TripleDashPause    = 0.15f; // pause entre les 3 dashes
    public float TripleDashRecovery = 0.6f;

    // ── Recovery ──────────────────────────────────────────────────────
    [Header("Recovery")]
    public float RecoveryDuration = 1.2f;

    // ── States ────────────────────────────────────────────────────────
    private EnemyStateBase summonState;
    private EnemyStateBase longDashState;
    private EnemyStateBase tripleDashState;
    private EnemyStateBase recoveryState;

    public EnemyStateBase GetRecoveryState() => recoveryState;

    private Pattern lastPattern;
    private enum Pattern { Summon, LongDash, TripleDash }
    
    protected override void Start()
    {
        base.Start();

        Debug.Log($"[{gameObject.name}] States → " +
                  $"Idle:{idleState != null}, " +
                  $"Chase:{chaseState != null}, " +
                  $"Attack:{attackState != null}, " +
                  $"Hurt:{hurtState != null}, " +
                  $"Death:{deathState != null}");
    }


    protected override void InitStates()
    {
        chaseState = null; // 👈 volontaire
        chaseRange  = 999f;
        attackRange = 999f;

        summonState     = new BossSummonState(this, StateMachine);
        longDashState   = new BossLongDashState(this, StateMachine);
        tripleDashState = new BossTripleDashState(this, StateMachine);
        recoveryState   = new BossRecoveryState(this, StateMachine,
                              RecoveryDuration, SelectNextPattern);
        hurtState       = new HurtState(this, StateMachine, 0.1f);
        deathState      = new DeathState(this, StateMachine);

        idleState = recoveryState;
    }

    public override void PerformAttack() { }

    public void SelectNextPattern()
    {
        Pattern next;
        do { next = (Pattern)Random.Range(0, 3); }
        while (next == lastPattern);

        lastPattern = next;

        switch (next)
        {
            case Pattern.Summon:     StateMachine.ChangeState(summonState);     break;
            case Pattern.LongDash:   StateMachine.ChangeState(longDashState);   break;
            case Pattern.TripleDash: StateMachine.ChangeState(tripleDashState); break;
        }
    }

    public override void Die()
    {
        Debug.Log("Boss 3 mort !");
        base.Die();
    }
}