// Scripts/Player/PlayerHealth.cs
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent               onDeath;


    private PlayerStats stats;
    private float lastMaxHealth;
    private float       currentHealth;
    private bool        isDead;
    private bool        isInvincible = false;
    private bool        isMirror     = false;

    public bool IsDead => isDead;

    // ─── Invincibilité / Miroir ───────────────────────────────────────────────

    public void SetInvincible(bool value)
    {
        isInvincible = value;
        Debug.Log($"🛡️ Invincible : {value}");
    }

    public void SetMirror(bool value)
    {
        isMirror = value;
        Debug.Log($"🪞 Miroir : {value}");
    }

    // ─── IDamageable ─────────────────────────────────────────────────────────

    public bool IsDead_ => isDead;

    public void TakeDamage(float amount)
    {
        if (isDead)       return;
        if (isInvincible) return;

        float tankiness   = stats.GetStat(StatType.Tankiness);
        float finalDamage = amount * (1f - Mathf.Clamp01(tankiness));

        // Renvoi des dégâts si miroir actif
        if (isMirror)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 15f);
            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                IDamageable enemy = hit.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(finalDamage);
                    Debug.Log($"🪞 Dégâts renvoyés à {hit.name} : {finalDamage}");
                }
                break;
            }
            return; // Ne prend pas les dégâts
        }

        currentHP -= finalDamage;
        currentHP  = Mathf.Max(currentHP, 0f);

        onHealthChanged?.Invoke(currentHealth, stats.GetStat(StatType.MaxHealth));

        if (currentHealth <= 0f)
            Die();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public void Heal(float amount)
    {
        if (isDead) return;

        float maxHP = stats.GetStat(StatType.MaxHealth);
        currentHealth   = Mathf.Min(currentHealth + amount, maxHP);

        onHealthChanged?.Invoke(currentHealth, maxHP);
    }

    public float GetCurrentHP() => currentHealth;
    public float GetMaxHP()     => stats != null ? stats.GetStat(StatType.MaxHealth) : 100f;
    public float GetHPRatio()   => currentHealth / GetMaxHP();

    // ─── Unity ───────────────────────────────────────────────────────────────

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }


    void Start()
    {
        float maxHP = stats.GetStat(StatType.MaxHealth);
        currentHealth = maxHP;
        lastMaxHealth = maxHP;

        Invoke(nameof(BroadcastHP), 0.1f);
    }


    void BroadcastHP()
    {
        onHealthChanged?.Invoke(currentHealth, stats.GetStat(StatType.MaxHealth));
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        onDeath?.Invoke();
        gameObject.SetActive(false);
    }
    
    void OnEnable()
    {
        stats = GetComponent<PlayerStats>();
        if (stats != null)
            stats.OnStatsChanged += HandleStatsChanged;
    }

    void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= HandleStatsChanged;
    }
    void HandleStatsChanged()
    {
        float previousMaxHP = lastMaxHealth;
        float newMaxHP = stats.GetStat(StatType.MaxHealth);

        float delta = newMaxHP - previousMaxHP;

        // ✅ Roguelike-friendly : la vie augmente avec le max
        if (delta > 0)
            currentHealth += delta;

        // ✅ Clamp de sécurité
        currentHealth = Mathf.Min(currentHealth, newMaxHP);

        lastMaxHealth = newMaxHP;

        // ✅ Notify l’UI
        onHealthChanged?.Invoke(currentHealth, newMaxHP);
    }

}