using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 10;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Détruire après un certain temps
        Destroy(gameObject, lifeTime);

        // Appliquer la vitesse
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Player hit");

            // Ici tu pourras appeler un script de vie plus tard
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);

            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}