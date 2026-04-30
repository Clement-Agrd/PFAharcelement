using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Bestiaire")]
    public BestiaryEntry bestiaryEntry;

    float currentHealth;
    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} prend {damage} dégâts");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public bool IsDead => isDead;

    void Die()
    {
        isDead = true;

        Debug.Log($"{gameObject.name} est mort");

        // Débloque la créature dans le bestiaire
        if (bestiaryEntry != null)
        {
            BestiaryManager.Instance.UnlockCreature(bestiaryEntry.id);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}