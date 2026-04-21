// Scripts/Projectile/Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifeStealRatio;

    public float lifetime = 5f;

    int playerLayer;
    int playerHitboxLayer;

    void Awake()
    {
        playerLayer        = LayerMask.NameToLayer("Player");
        playerHitboxLayer  = LayerMask.NameToLayer("PlayerHitbox");
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ✅ Ignore joueur + hitbox joueur
        if (other.gameObject.layer == playerLayer ||
            other.gameObject.layer == playerHitboxLayer)
        {
            return;
        }

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);

            // Vol de vie
            if (lifeStealRatio > 0f)
            {
                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(damage * lifeStealRatio);
                }
            }
        }

        Destroy(gameObject);
    }
}
