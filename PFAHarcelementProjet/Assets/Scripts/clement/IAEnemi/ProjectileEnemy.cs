using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 10;

    int playerHitboxLayer;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerHitboxLayer = LayerMask.NameToLayer("PlayerHitbox");
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ✅ HITBOX JOUEUR
        if (other.gameObject.layer == playerHitboxLayer)
        {
            // 🔥 On remonte jusqu'au PlayerHealth
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

        // ✅ Décors
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }
}