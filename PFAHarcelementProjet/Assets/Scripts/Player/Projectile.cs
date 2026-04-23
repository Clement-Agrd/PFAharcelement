// Scripts/Projectile/Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifeStealRatio;
    
    
    [Header("Homing")]
    public float detectRadius = 6f;
    public float minTurnSpeed = 1f;
    public float maxTurnSpeed = 8f;

    
    private Transform target;
    private Vector3 currentDirection;


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
        currentDirection = transform.forward;
    }


    void FindNearbyTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius);
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }

        target = closest;
    }

    

    void Update()
    {
        // Recherche continue de cible
        FindNearbyTarget();
        
        if (target != null)
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, target.position);

            // Plus c'est proche → plus ça tourne fort
            float t = Mathf.InverseLerp(detectRadius, 0f, dist);
            float turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, t);

            currentDirection = Vector3.Slerp(
                currentDirection,
                toTarget,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position += currentDirection * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(currentDirection);
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
            Destroy(gameObject);
        }
    }
}
