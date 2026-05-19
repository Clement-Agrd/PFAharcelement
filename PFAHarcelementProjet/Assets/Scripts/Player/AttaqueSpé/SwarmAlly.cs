// Scripts/Ultimates/SwarmAlly.cs
using UnityEngine;

public class SwarmAlly : MonoBehaviour
{
    private Transform target;
    private float     damage;
    private float     lifetime;
    private float     elapsed;
    private float     orbitRadius    = 3f;
    private float     orbitSpeed     = 90f;
    private float     attackRange    = 2f;
    private float     attackCooldown = 1f;
    private float     nextAttack;
    private float     angle;

    public void Setup(Transform playerTransform, float dmg, float life)
    {
        target   = playerTransform;
        damage   = dmg;
        lifetime = life;
        angle    = Random.Range(0f, 360f);

        // Ajoute le VFX sur l'allié
        SwarmAllyVFX vfx = GetComponent<SwarmAllyVFX>();
        if (vfx == null)
            vfx = gameObject.AddComponent<SwarmAllyVFX>();
    }

    void Update()
    {
        if (target == null) return;

        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        angle += orbitSpeed * Time.deltaTime;
        float rad      = angle * Mathf.Deg2Rad;
        Vector3 orbitPos = target.position + new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            0f,
            Mathf.Sin(rad) * orbitRadius
        );
        transform.position = orbitPos;
        transform.LookAt(new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z
        ));

        if (Time.time >= nextAttack)
            AttackNearestEnemy();
    }

    void AttackNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"🦈 Allié attaque {hit.name} : {damage} dégâts");
            }

            nextAttack = Time.time + attackCooldown;
            break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}