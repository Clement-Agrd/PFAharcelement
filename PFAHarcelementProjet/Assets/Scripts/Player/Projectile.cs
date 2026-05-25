// Scripts/Projectile/Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifeStealRatio;
    [HideInInspector] public bool  isReflected = false; // ← nouveau

    [Header("Homing")]
    public float detectRadius = 6f;
    public float minTurnSpeed = 1f;
    public float maxTurnSpeed = 8f;

    private Transform target;
    private Vector3   currentDirection;

    public float lifetime = 5f;

    int playerLayer;
    int playerHitboxLayer;
    int invisibleLayer;

    void Awake()
    {
        playerLayer       = LayerMask.NameToLayer("Player");
        playerHitboxLayer = LayerMask.NameToLayer("PlayerHitbox");
        invisibleLayer    = LayerMask.NameToLayer("Invisible");
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
        currentDirection = transform.forward;
    }

    void FindNearbyTarget()
    {
        // Si renvoyé cherche des ennemis au lieu du joueur
        string targetTag = isReflected ? "Enemy" : "Enemy";

        Collider[] hits       = Physics.OverlapSphere(transform.position, detectRadius);
        float      closestDist = Mathf.Infinity;
        Transform  closest    = null;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest     = hit.transform;
            }
        }

        target = closest;
    }

    void Update()
    {
        FindNearbyTarget();

        if (target != null)
        {
            Vector3 toTarget  = (target.position - transform.position).normalized;
            float   dist      = Vector3.Distance(transform.position, target.position);
            float   t         = Mathf.InverseLerp(detectRadius, 0f, dist);
            float   turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, t);

            currentDirection = Vector3.Slerp(
                currentDirection,
                toTarget,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position += currentDirection * speed * Time.deltaTime;
        transform.rotation  = Quaternion.LookRotation(currentDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        int hitLayer = other.gameObject.layer;

        // Ignore joueur et layers associés si pas renvoyé
        if (!isReflected)
        {
            if (hitLayer == playerLayer       ||
                hitLayer == playerHitboxLayer ||
                hitLayer == invisibleLayer)
                return;

            if (IsPlayerInvisible(other.gameObject)) return;
        }
        else
        {
            // Renvoyé — ignore les autres projectiles et le miroir
            if (other.CompareTag("EnemyProjectile")) return;
            if (other.GetComponent<MirrorDisk>() != null) return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (lifeStealRatio > 0f)
            {
                PlayerHealth ph = FindObjectOfType<PlayerHealth>();
                if (ph != null) ph.Heal(damage * lifeStealRatio);
            }

            Destroy(gameObject);
        }
    }

    bool IsPlayerInvisible(GameObject go)
    {
        Transform current = go.transform;
        while (current != null)
        {
            if (current.gameObject.layer == invisibleLayer)
                return true;
            current = current.parent;
        }
        return false;
    }
}