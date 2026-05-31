// Scripts/Player/PlayerHealth.cs

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDeath;

    [Header("Mort")]
    public float deathDelay = 2f;

    public static Action OnPlayerDamaged;

    private PlayerStats stats;
    private float lastMaxHealth;
    private float currentHealth;

    private bool isDead;
    private bool isInvincible = false;
    private bool isMirror = false;

    public bool IsDead => isDead;

    // ─────────────────────────────────────────────────────────────
    // INVINCIBILITÉ / MIROIR
    // ─────────────────────────────────────────────────────────────

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

    // ─────────────────────────────────────────────────────────────
    // DAMAGE
    // ─────────────────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (isInvincible) return;

        float tankiness = stats.GetStat(StatType.Tankiness);
        float finalDamage = amount * (1f - Mathf.Clamp01(tankiness));

        // Renvoi des dégâts
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

            return;
        }

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        OnPlayerDamaged?.Invoke();

        onHealthChanged?.Invoke(currentHealth, stats.GetStat(StatType.MaxHealth));

        if (currentHealth <= 0f)
            Die();
    }

    // ─────────────────────────────────────────────────────────────
    // HEAL
    // ─────────────────────────────────────────────────────────────

    public void Heal(float amount)
    {
        if (isDead) return;

        float maxHP = stats.GetStat(StatType.MaxHealth);

        currentHealth = Mathf.Min(currentHealth + amount, maxHP);

        onHealthChanged?.Invoke(currentHealth, maxHP);
    }

    public float GetCurrentHP() => currentHealth;
    public float GetMaxHP() => stats != null ? stats.GetStat(StatType.MaxHealth) : 100f;
    public float GetHPRatio() => currentHealth / GetMaxHP();

    // ─────────────────────────────────────────────────────────────
    // UNITY
    // ─────────────────────────────────────────────────────────────

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        ResetPlayer();
    }

    void OnEnable()
    {
        if (stats != null)
            stats.OnStatsChanged += HandleStatsChanged;
    }

    void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= HandleStatsChanged;
    }

    // ─────────────────────────────────────────────────────────────
    // RESET PLAYER
    // ─────────────────────────────────────────────────────────────

    public void ResetPlayer()
    {
        isDead = false;

        float maxHP = stats.GetStat(StatType.MaxHealth);

        currentHealth = maxHP;
        lastMaxHealth = maxHP;

        onHealthChanged?.Invoke(currentHealth, maxHP);
    }

    // ─────────────────────────────────────────────────────────────
    // STATS CHANGED
    // ─────────────────────────────────────────────────────────────

    void HandleStatsChanged()
    {
        float newMaxHP = stats.GetStat(StatType.MaxHealth);

        float delta = newMaxHP - lastMaxHealth;

        if (delta > 0)
            currentHealth += delta;

        currentHealth = Mathf.Min(currentHealth, newMaxHP);

        lastMaxHealth = newMaxHP;

        onHealthChanged?.Invoke(currentHealth, newMaxHP);
    }

    // ─────────────────────────────────────────────────────────────
    // DEATH
    // ─────────────────────────────────────────────────────────────

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("💀 Joueur mort");

        onDeath?.Invoke();

        // Convertit le gold en XP
        if (XPManager.Instance != null)
            XPManager.Instance.ConvertGoldToXP();

        Invoke(nameof(ReturnToMenu), deathDelay);
    }

    // ─────────────────────────────────────────────────────────────
    // MENU RESET
    // ─────────────────────────────────────────────────────────────

    void ReturnToMenu()
    {
        ResetRun(gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    // ─────────────────────────────────────────────────────────────
    // RESET RUN
    // ─────────────────────────────────────────────────────────────

    void ResetRun(GameObject player)
    {
        // Reset buffs
        PlayerStats stats = player.GetComponent<PlayerStats>();

        if (stats != null)
            stats.ClearAllModifiers();

        // Reset HP
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null)
            health.ResetPlayer();

        // Reset ultimate
        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);
        
        stats.ResetRunStats();

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);

        // Désactive le player dans le menu
        player.SetActive(false);

        // Reset position
        player.transform.position = Vector3.zero;
    }
    
}