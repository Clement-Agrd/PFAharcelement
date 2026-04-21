using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;

    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} prend {damage} dégâts");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public bool IsDead { get; }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort");
        Destroy(gameObject);
    }

    // Gizmo debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}