// Scripts/Enemies/EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;

    private float   currentHealth;
    private bool    isDead = false;

    public bool IsDead => isDead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth  = Mathf.Max(currentHealth, 0f);

        Debug.Log($"💢 {gameObject.name} : {currentHealth}/{maxHealth} HP");

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Passe par EnemyController pour le gold et le bestiaire
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
            controller.Die();
        else
        {
            // Fallback si pas de EnemyController
            if (XPManager.Instance != null)
                Debug.Log("⚠️ EnemyController introuvable — gold non donné");

            Debug.Log($"☠️ {gameObject.name} mort");
            Destroy(gameObject);
        }
    }
}