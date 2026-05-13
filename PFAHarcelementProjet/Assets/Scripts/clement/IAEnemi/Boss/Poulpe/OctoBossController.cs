using UnityEngine;

public class OctoBossController : EnemyController
{
    // ── Pattern 1 — Tentacules ────────────────────────────────────────
    [Header("Tentacle Pattern")]
    public GameObject TentaclePrefab;
    public float      TentacleSpawnRadius = 7f;
    public float      WaveInterval        = 2.5f;

    // ── Pattern 2 — Ink ───────────────────────────────────────────────
    [Header("Ink Pattern")]
    public GameObject  InkProjectilePrefab;
    public Transform[] InkFirePoints;
    public int         InkProjectileCount = 16;
    public float       InkFireInterval    = 0.08f;

    // ── Pattern 3 — Sweep ─────────────────────────────────────────────
    [Header("Sweep Pattern")]
    public GameObject    SweepTentaclePrefab;
    public OctoSweepLine[] SweepLines;
    public float         SweepStaggerDelay  = 0.15f;
    public float         SweepTotalDuration = 3f;

    // ── Telegraph ─────────────────────────────────────────────────────
    [Header("Telegraph")]
    public VFXPlayer TelegraphVFX;          // VFX d'indication avant chaque pattern
    public float     TelegraphDuration = 1f; // durée de l'anim Attack avant le pattern

    // ── Recovery ──────────────────────────────────────────────────────
    [Header("Recovery")]
    public float RecoveryDuration = 1.5f;

    // ── States ────────────────────────────────────────────────────────
    private EnemyStateBase tentacleState;
    private EnemyStateBase inkState;
    private EnemyStateBase sweepState;
    private EnemyStateBase recoveryState;

    public EnemyStateBase GetRecoveryState()   => recoveryState;
    public EnemyStateBase GetTentacleState()   => tentacleState;
    public EnemyStateBase GetInkState()        => inkState;
    public EnemyStateBase GetSweepState()      => sweepState;

    private Pattern lastPattern;
    private enum Pattern { Tentacle, Ink, Sweep }

    protected override void InitStates()
    {
        chaseRange  = 999f;
        attackRange = 999f;

        tentacleState = new OctoTentacleState(this, StateMachine);
        inkState      = new OctoInkState(this, StateMachine);
        sweepState    = new OctoSweepState(this, StateMachine);
        recoveryState = new BossRecoveryState(this, StateMachine,
                            RecoveryDuration, SelectNextPattern);
        hurtState     = new HurtState(this, StateMachine, 0.1f);
        deathState    = new DeathState(this, StateMachine);

        idleState = recoveryState;
    }

    public override void PerformAttack() { }

    public void SelectNextPattern()
    {
        Pattern next;
        do { next = (Pattern)Random.Range(0, 3); }
        while (next == lastPattern);

        lastPattern = next;

        // Passe d'abord par le telegraph avant chaque pattern
        StateMachine.ChangeState(new OctoTelegraphState(this, StateMachine, next));
    }

    public void LaunchPattern(int patternIndex)
    {
        switch ((Pattern)patternIndex)
        {
            case Pattern.Tentacle: StateMachine.ChangeState(tentacleState); break;
            case Pattern.Ink:      StateMachine.ChangeState(inkState);      break;
            case Pattern.Sweep:    StateMachine.ChangeState(sweepState);    break;
        }
    }

    public override void Die()
    {
        Debug.Log("Pieuvre morte !");
        base.Die();
    }
}