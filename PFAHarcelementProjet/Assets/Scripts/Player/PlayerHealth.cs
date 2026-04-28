// Scripts/Player/PlayerHealth.cs
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent               onDeath;

    private PlayerStats stats;
    private float       currentHP;
    private bool        isDead;

    public bool IsDead => isDead;

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        float tankiness   = stats.GetStat(StatType.Tankiness);
        float finalDamage = amount * (1f - Mathf.Clamp01(tankiness));

        currentHP -= finalDamage;
        currentHP  = Mathf.Max(currentHP, 0f);

        onHealthChanged?.Invoke(currentHP, stats.GetStat(StatType.HP));

        if (currentHP <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        float maxHP = stats.GetStat(StatType.HP);
        currentHP   = Mathf.Min(currentHP + amount, maxHP);

        onHealthChanged?.Invoke(currentHP, maxHP);
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP()     => stats != null ? stats.GetStat(StatType.HP) : 100f;
    public float GetHPRatio()   => currentHP / GetMaxHP();

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        currentHP = stats.GetStat(StatType.HP);
        // Petit délai pour s'assurer que HealthBarUI est bien initialisé
        Invoke(nameof(BroadcastHP), 0.1f);
    }

    void BroadcastHP()
    {
        onHealthChanged?.Invoke(currentHP, stats.GetStat(StatType.HP));
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        onDeath?.Invoke();
        gameObject.SetActive(false);
    }
}