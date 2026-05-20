using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public abstract class EnemyController : MonoBehaviour, IDamageable
{
    public bool AllowRotation = true;
    
    [Header("Rotation")]
    public float rotationSpeed = 10f;
    
    [Header("Bestiaire")]
    public BestiaryEntry bestiaryEntry;

    [Header("Stats")]
    public float maxHealth   = 100f;
    public float moveSpeed   = 3f;
    public float attackRange = 8f;
    public float chaseRange  = 20f;
    public float attackCooldown = 1.2f;
    public float stopChaseRange = 4f; // Distance à laquelle l'ennemi arrête de chase
    
    public abstract void PerformAttack();
    
    public bool IsDead => CurrentHealth <= 0f;

    // Runtime
    public float          CurrentHealth { get; private set; }
    public Transform      PlayerTransform { get; private set; }
    public Rigidbody    Rb            { get; private set; }
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
        Rb           = GetComponent<Rigidbody>();
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
    
    // ✅ EnemyController.cs — ajoute cette méthode
    public void PlayAnim(string stateName)
    {
        if (Anim == null) return;
        if (!Anim.isActiveAndEnabled) return;
        if (Anim.HasState(0, Animator.StringToHash(stateName)))
            Anim.Play(stateName);
    }

// ✅ Ajoute ici
    public void TriggerAnim(string triggerName)
    {
        if (Anim == null) return;
        if (!Anim.isActiveAndEnabled) return;
        Anim.SetTrigger(triggerName);
    }
    
    protected virtual void FixedUpdate()  => StateMachine.FixedUpdate();

    // Chaque ennemi crée ses propres états ici
    protected abstract void InitStates();

    // ── API publique ─────────────────────────────────────────────────
    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return float.MaxValue;
        return Vector3.Distance(transform.position, PlayerTransform.position);
    }

    public bool IsPlayerInRange(float range) => DistanceToPlayer() <= range;

    public virtual void TakeDamage(float amount)
    {
        if (StateMachine.CurrentState == deathState) return;

        CurrentHealth -= amount;

        Debug.Log($"{gameObject.name} prend {amount} dégâts");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            StateMachine.ChangeState(deathState);
        }
        else
        {
            StateMachine.ChangeState(hurtState);
        }
    }
    
    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} est mort");

        if (bestiaryEntry != null)
        {
            BestiaryManager.Instance.UnlockCreature(bestiaryEntry.id);
        }

        gameObject.SetActive(false); // ✅ important
        Destroy(gameObject, 2f);
    }
    protected virtual void Update()
    {
        StateMachine.Update();

        RotateTowardsPlayer();
    }
    protected virtual void RotateTowardsPlayer()
    {
        if (!AllowRotation) return;
        if (PlayerTransform == null) return;
        if (IsDead) return;

        Vector3 dir = PlayerTransform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}