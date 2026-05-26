// Scripts/Player/PlayerHealth.cs
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent               onDeath;

    [Header("Mort")]
    public float deathDelay = 2f; // délai avant retour menu

    public static Action OnPlayerDamaged;

    private PlayerStats stats;
    private float       lastMaxHealth;
    private float       currentHealth;
    private bool        isDead;
    private bool        isInvincible = false;
    private bool        isMirror     = false;

    public bool IsDead  => isDead;
    public bool IsDead_ => isDead;

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
            return;
        }

        currentHealth -= finalDamage;
        currentHealth  = Mathf.Max(currentHealth, 0f);

        OnPlayerDamaged?.Invoke();

        onHealthChanged?.Invoke(currentHealth, stats.GetStat(StatType.MaxHealth));

        if (currentHealth <= 0f)
            Die();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public void Heal(float amount)
    {
        if (isDead) return;

        float maxHP   = stats.GetStat(StatType.MaxHealth);
        currentHealth = Mathf.Min(currentHealth + amount, maxHP);

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
        float maxHP   = stats.GetStat(StatType.MaxHealth);
        currentHealth = maxHP;
        lastMaxHealth = maxHP;

        Invoke(nameof(BroadcastHP), 0.1f);
    }

    void BroadcastHP()
    {
        onHealthChanged?.Invoke(currentHealth, stats.GetStat(StatType.MaxHealth));
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
        float newMaxHP = stats.GetStat(StatType.MaxHealth);
        float delta    = newMaxHP - lastMaxHealth;

        if (delta > 0)
            currentHealth += delta;

        currentHealth = Mathf.Min(currentHealth, newMaxHP);
        lastMaxHealth = newMaxHP;

        onHealthChanged?.Invoke(currentHealth, newMaxHP);
    }

    // ─── Mort ────────────────────────────────────────────────────────────────

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Joueur mort");

        onDeath?.Invoke();

        // Convertit le gold en XP
        if (XPManager.Instance != null)
        {
            XPManager.Instance.ConvertGoldToXP();
        }

        // Retour au menu après délai
        Invoke(nameof(CleanupAndLoad), deathDelay);
    }

    void CleanupAndLoad()
    {
        // ✅ récupère tous les objets, même persistants
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // ✅ skip ceux qui sont dans un asset/prefab
            if (!obj.scene.IsValid()) continue;

            // ✅ skip GameManagers (tagged Persistent)
            if (obj.CompareTag("Persistent")) continue;

            // ✅ on ne détruit pas le portail lui-même
            if (obj == gameObject) continue;

            Destroy(obj);
        }

        SceneManager.LoadScene("MainMenu");
    }
}