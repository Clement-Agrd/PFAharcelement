// Scripts/Player/PlayerHealth.cs
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged; // (hpActuel, hpMax)
    public UnityEvent               onDeath;

    private PlayerStats stats;
    private float       currentHP;
    private bool        isDead;

    // ─── IDamageable ─────────────────────────────────────────────────────────

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

    // ─── API publique ─────────────────────────────────────────────────────────

    public void Heal(float amount)
    {
        if (isDead) return;

        float maxHP = stats.GetStat(StatType.HP);
        currentHP   = Mathf.Min(currentHP + amount, maxHP);

        onHealthChanged?.Invoke(currentHP, maxHP);
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP()     => stats.GetStat(StatType.HP);
    public float GetHPRatio()   => currentHP / stats.GetStat(StatType.HP);

    // ─── Mort ────────────────────────────────────────────────────────────────

    void Die()
    {
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();

        // Tu peux ajouter ici : animation de mort, désactiver les inputs, etc.
        gameObject.SetActive(false);
    }

    // ─── Unity ───────────────────────────────────────────────────────────────

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        currentHP = stats.GetStat(StatType.HP);
        onHealthChanged?.Invoke(currentHP, currentHP);
    }
}