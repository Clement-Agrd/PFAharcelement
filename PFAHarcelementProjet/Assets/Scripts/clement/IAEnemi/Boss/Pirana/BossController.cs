using UnityEngine;

public class BossController : EnemyController
{
    [Header("Swarm")]
    public ParticleSystem PiranaSwarm;          // l'amas principal
    public VFXPlayer TelegraphVFX;    // ring / glow au sol avant chaque attaque

    [Header("Telegraph")]
    public float TelegraphDuration     = 1.2f;  // durée de l'indication
    public float TelegraphNoiseStrength = 1.5f; // agitation du swarm pendant le telegraph
    public float TelegraphEmissionRate  = 60f;  // piranhas en plus pendant le telegraph
    
    // ── Pattern Selection ─────────────────────────────────────────────
    private enum Pattern { Spin, Zone, Burst }
    private Pattern lastPattern;

    // ── Refs ──────────────────────────────────────────────────────────
    [Header("Refs")]
    public Transform  FirePoint;
    public GameObject ProjectilePrefab;
    public GameObject ZonePrefab;
    // ── Spin ──────────────────────────────────────────────────────────
    [Header("Spin Attack")]
    public float SpinSpeed     = 360f;
    public float SpinDuration  = 1.5f;
    public float ExpandDuration = 0.8f;
    public float AoERadius     = 5f;
    public float AoEDamagePerSecond = 40f;

    // ── Zones ─────────────────────────────────────────────────────────
    [Header("Zone Attack")]
    public int   RandomZoneCount  = 4;
    public float ZoneSpawnRadius  = 8f;
    public float ZoneStateDuration = 3.5f; // durée totale de l'état (zones + attente)

    // ── Burst ─────────────────────────────────────────────────────────
    [Header("Burst Attack")]
    public int   BurstCount    = 8;
    public float BurstInterval = 0.15f;
    public float BurstSpread   = 0.08f;

    // ── Recovery ──────────────────────────────────────────────────────
    [Header("Recovery")]
    public float RecoveryDuration = 1.2f;

    // ── States ────────────────────────────────────────────────────────
    private EnemyStateBase spinState;
    private EnemyStateBase zoneState;
    private EnemyStateBase burstState;
    private EnemyStateBase recoveryState;

    public EnemyStateBase GetRecoveryState() => recoveryState;

    protected override void InitStates()
    {
        chaseRange  = 999f; // boss présent dans toute la room
        attackRange = 999f;

        spinState     = new BossSpinState(this, StateMachine);
        zoneState     = new BossZoneState(this, StateMachine);
        burstState    = new BossBurstState(this, StateMachine);
        recoveryState = new BossRecoveryState(this, StateMachine,
            RecoveryDuration, SelectNextPattern);
        hurtState     = new HurtState(this, StateMachine, 0.1f);
        deathState    = new DeathState(this, StateMachine);

        // Le boss démarre par la Recovery (courte intro avant le premier pattern)
        idleState = recoveryState;
    }

    public override void PerformAttack() { } // géré par les states

    // Appelé par BossRecoveryState à la fin de chaque pause
    public void SelectNextPattern()
    {
        // Ne répète jamais le même pattern deux fois de suite
        Pattern next;
        do { next = (Pattern)Random.Range(0, 3); }
        while (next == lastPattern);

        lastPattern = next;

        switch (next)
        {
            case Pattern.Spin:  StateMachine.ChangeState(spinState);  break;
            case Pattern.Zone:  StateMachine.ChangeState(zoneState);  break;
            case Pattern.Burst: StateMachine.ChangeState(burstState); break;
        }
    }

    public override void Die()
    {
        Debug.Log("Boss mort — spawn récompense !");
        base.Die();
    }
}