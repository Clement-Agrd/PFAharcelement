// Scripts/Enemies/ProjectileEnemy.cs
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed    = 10f;
    public float lifeTime = 5f;
    public int   damage   = 10;

    // ← ajouts pour le renvoi
    [HideInInspector] public bool    isReflected      = false;
    [HideInInspector] public Vector3 reflectedDirection = Vector3.zero;

    int       playerHitboxLayer;
    Rigidbody rb;

    void Awake()
    {
        rb                = GetComponent<Rigidbody>();
        playerHitboxLayer = LayerMask.NameToLayer("PlayerHitbox");
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Si renvoyé on gère le mouvement manuellement
        if (isReflected && reflectedDirection != Vector3.zero)
        {
            transform.position += reflectedDirection * speed * Time.deltaTime;
            transform.rotation  = Quaternion.LookRotation(reflectedDirection);
        }
    }

    public void ResetDirection(Vector3 newDirection)
    {
        reflectedDirection  = newDirection.normalized;
        transform.forward   = reflectedDirection;

        // Arrête le Rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic    = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ─── Projectile renvoyé ───────────────────────────────────────────
        if (isReflected)
        {
            if (other.gameObject.layer == playerHitboxLayer) return;
            if (other.GetComponent<MirrorDisk>() != null)    return;

            if (other.CompareTag("Enemy"))
            {
                IDamageable target = other.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                    Debug.Log($"🪞 Dégâts sur {other.name} : {damage}");
                }
                Destroy(gameObject);
                return;
            }

            if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
                return;
            }

            return;
        }

        // ─── Projectile normal ────────────────────────────────────────────
        if (other.gameObject.layer == playerHitboxLayer)
        {
            PlayerHealth playerHealth =
                other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"💥 Player hit for {damage} damage");
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Init(Vector3 direction)
    {
        if (rb != null)
            rb.linearVelocity = direction.normalized * speed;
    }
}