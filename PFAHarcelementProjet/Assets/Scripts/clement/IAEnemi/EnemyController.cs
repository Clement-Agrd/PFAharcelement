using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth   = 100f;
    public float moveSpeed   = 3f;
    public float attackRange = 1.5f;
    public float chaseRange  = 8f;
    public float attackCooldown = 1.2f;
    
    
    public abstract void PerformAttack();

    // Runtime
    public float          CurrentHealth { get; private set; }
    public Transform      PlayerTransform { get; private set; }
    public Rigidbody2D    Rb            { get; private set; }
    public Animator       Anim          { get; private set; }
    public StateMachine   StateMachine  { get; private set; }

    // États communs (surchargeables)
    protected EnemyStateBase idleState;
    protected EnemyStateBase chaseState;
    protected EnemyStateBase attackState;
    protected EnemyStateBase hurtState;
    protected EnemyStateBase deathState;


    public EnemyStateBase GetIdleState()   => idleState;
    public EnemyStateBase GetChaseState()  => chaseState;
    public EnemyStateBase GetAttackState() => attackState;
    public EnemyStateBase GetHurtState()   => hurtState;
    public EnemyStateBase GetDeathState()  => deathState;

    protected virtual void Awake()
    {
        Rb           = GetComponent<Rigidbody2D>();
        Anim         = GetComponent<Animator>();
        StateMachine = new StateMachine();
        CurrentHealth = maxHealth;

        // Le joueur doit avoir le tag "Player"
        var player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;

        InitStates();
    }

    protected virtual void Start()
    {
        StateMachine.Initialize(idleState);
    }

    protected virtual void Update()       => StateMachine.Update();
    protected virtual void FixedUpdate()  => StateMachine.FixedUpdate();

    // Chaque ennemi crée ses propres états ici
    protected abstract void InitStates();

    // ── API publique ─────────────────────────────────────────────────
    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return float.MaxValue;
        return Vector2.Distance(transform.position, PlayerTransform.position);
    }

    public bool IsPlayerInRange(float range) => DistanceToPlayer() <= range;

    public virtual void TakeDamage(float amount)
    {
        if (StateMachine.CurrentState == deathState) return;

        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
            StateMachine.ChangeState(deathState);
        else
            StateMachine.ChangeState(hurtState);
    }

    public virtual void Die()
    {
        // Override pour loot, VFX supplémentaires...
        Destroy(gameObject, 2f);
    }
}