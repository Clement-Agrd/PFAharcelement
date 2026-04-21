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
        // ✅ HITBOX JOUEUR (pas le CharacterController)
        if (other.gameObject.layer == playerHitboxLayer)
        {
            Debug.Log("💥 Player hit (hitbox)");

            // Exemple plus tard :
            // other.GetComponentInParent<PlayerHealth>()?.TakeDamage(damage);

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