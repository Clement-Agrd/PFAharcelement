// Scripts/Enemies/EnemyController.cs
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
    public float maxHealth      = 100f;
    public float moveSpeed      = 3f;
    public float attackRange    = 8f;
    public float chaseRange     = 20f;
    public float attackCooldown = 1.2f;
    public float stopChaseRange = 4f;

    [Header("Récompense")]
    public int goldValue = 10;
    public int xpValue   = 5;
    
    public abstract void PerformAttack();

    public bool IsDead => CurrentHealth <= 0f;

    public float        CurrentHealth   { get; private set; }
    public Transform    PlayerTransform { get; private set; }
    public Rigidbody    Rb              { get; private set; }
    public Animator     Anim            { get; private set; }
    public StateMachine StateMachine    { get; private set; }

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
        Anim = GetComponentInChildren<Animator>();
        StateMachine = new StateMachine();
        CurrentHealth = maxHealth;

        var player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;

        InitStates();
    }

    public bool IsPlayerInvisible()
    {
        if (PlayerTransform == null) return false;
        return PlayerTransform.gameObject.layer ==
               LayerMask.NameToLayer("Invisible");
    }

    protected virtual void Start()
    {
        StateMachine.Initialize(idleState);
    }

    public void PlayAnim(string stateName)
    {
        if (Anim == null) return;
        if (!Anim.isActiveAndEnabled) return;
        if (Anim.HasState(0, Animator.StringToHash(stateName)))
            Anim.Play(stateName);
    }

    public void TriggerAnim(string triggerName)
    {
        if (Anim == null)
        {
            Debug.LogError($"❌ Animator NULL sur {gameObject.name} !");
            return;
        }

        if (!Anim.isActiveAndEnabled)
        {
            Debug.LogWarning($"⚠️ Animator désactivé sur {gameObject.name}");
            return;
        }

        // Vérifie si le param existe
        if (!HasParameter(triggerName))
        {
            Debug.LogError($"❌ Trigger '{triggerName}' n'existe PAS sur Animator de {gameObject.name}");
            return;
        }

        Debug.Log($"✅ Trigger '{triggerName}' envoyé sur {gameObject.name}");

        Anim.ResetTrigger(triggerName); // évite les blocages
        Anim.SetTrigger(triggerName);
    }
    
    bool HasParameter(string paramName)
    {
        foreach (var param in Anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }


    protected virtual void FixedUpdate()  => StateMachine.FixedUpdate();

    protected virtual void FixedUpdate() => StateMachine.FixedUpdate();

    protected abstract void InitStates();

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

        if (XPManager.Instance != null)
        {
            // ✅ GOLD
            if (goldValue > 0)
            {
                XPManager.Instance.AddGold(goldValue);
                Debug.Log($"💰 +{goldValue} gold ({gameObject.name})");
            }

            // ✅ XP DIRECT
            if (xpValue > 0)
            {
                XPManager.Instance.AddXP(xpValue);
                Debug.Log($"✨ +{xpValue} XP ({gameObject.name})");
            }
        }

        // ─── Bestiaire ───────────────────
        if (bestiaryEntry != null)
            BestiaryManager.Instance.UnlockCreature(bestiaryEntry.id);

        gameObject.SetActive(false);
        Destroy(gameObject, 2f);
    }
    
    protected virtual void Update()
    {
        StateMachine.Update();
        RotateTowardsPlayer();
    }

    protected virtual void RotateTowardsPlayer()
    {
        if (!AllowRotation)    return;
        if (PlayerTransform == null) return;
        if (IsDead)            return;

        Vector3 dir = PlayerTransform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation   = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}