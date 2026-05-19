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
    public float GetMaxHP()     => stats != null ? stats.GetStat(StatType.HP) : 100f;
    public float GetHPRatio()   => currentHP / GetMaxHP();

    // ─── Unity ───────────────────────────────────────────────────────────────

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        currentHP = stats.GetStat(StatType.HP);
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