// Scripts/Player/PlayerHealth.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Événements")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent               onDeath;

    [Header("Mort")]
    public Image deathOverlay;

    public static Action OnPlayerDamaged;

    private PlayerStats stats;
    private float       lastMaxHealth;
    private float       currentHealth;
    private bool        isDead       = false;
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

    // ─── Damage ───────────────────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (isDead)       return;
        if (isInvincible) return;

        float tankiness   = stats.GetStat(StatType.Tankiness);
        float finalDamage = amount * (1f - Mathf.Clamp01(tankiness));

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

    // ─── Heal ─────────────────────────────────────────────────────────────────

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
        ResetPlayer();

        // S'assure que l'overlay est transparent au départ
        if (deathOverlay != null)
            deathOverlay.color = new Color(0f, 0f, 0f, 0f);
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

    // ─── Reset Player ─────────────────────────────────────────────────────────

    public void ResetPlayer()
    {
        isDead = false;

        if (stats == null)
            stats = GetComponent<PlayerStats>();

        float maxHP   = stats.GetStat(StatType.MaxHealth);
        currentHealth = maxHP;
        lastMaxHealth = maxHP;

        onHealthChanged?.Invoke(currentHealth, maxHP);

        // Reset overlay
        if (deathOverlay != null)
            deathOverlay.color = new Color(0f, 0f, 0f, 0f);
    }

    // ─── Stats Changed ────────────────────────────────────────────────────────

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

    // ─── Death ────────────────────────────────────────────────────────────────

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Joueur mort");

        onDeath?.Invoke();

        // Convertit le gold en XP
        if (XPManager.Instance != null)
        {
            Debug.Log($"✨ Conversion : {XPManager.Instance.GetGold()} gold → XP");
            XPManager.Instance.ConvertGoldToXP();
        }

        StartCoroutine(DeathFade());
    }

    IEnumerator DeathFade()
    {
        float elapsed  = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t  = Mathf.Clamp01(elapsed / duration);

            // Ralentit le temps progressivement
            Time.timeScale = Mathf.Lerp(1f, 0f, t);

            // Fade vers le noir
            if (deathOverlay != null)
                deathOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, t));

            yield return null;
        }

        // Stoppe complètement
        Time.timeScale = 0f;

        // Petite pause à l'écran noir
        yield return new WaitForSecondsRealtime(0.5f);

        ReturnToMenu();
    }

    // ─── Return To Menu ───────────────────────────────────────────────────────

    void ReturnToMenu()
    {
        ResetRun(gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ─── Reset Run ────────────────────────────────────────────────────────────

    void ResetRun(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.ClearAllModifiers();
            playerStats.ResetRunStats();
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.ResetPlayer();

        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);

        player.SetActive(false);
        player.transform.position = Vector3.zero;
    }
}