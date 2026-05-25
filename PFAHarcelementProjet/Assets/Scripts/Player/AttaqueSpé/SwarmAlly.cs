// Scripts/Ultimates/SwarmAlly.cs
using System.Collections;
using UnityEngine;

public class SwarmAlly : MonoBehaviour
{
    private Transform   playerTransform;
    private float       lifetime;
    private float       elapsed;
    private float       nextFire;
    private int         index;
    private int         totalCount;
    private float       spacing         = 2.5f;
    private GameObject  projectilePrefab;
    private PlayerStats playerStats;

    public void Setup(Transform player, float life,
                      int idx, int total,
                      GameObject projPrefab, PlayerStats stats)
    {
        playerTransform  = player;
        lifetime         = life;
        index            = idx;
        totalCount       = total;
        projectilePrefab = projPrefab;
        playerStats      = stats;
        spacing          = 2.5f;
    }

    void Update()
    {
        if (playerTransform == null) return;

        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        FollowFormation();

        // Cadence basée sur AttackSpeed + CooldownReduction du joueur
        float attackSpeed       = playerStats != null
            ? playerStats.GetStat(StatType.AttackSpeed)       : 4f;
        float cooldownReduction = playerStats != null
            ? playerStats.GetStat(StatType.CooldownReduction) : 0f;
        float fireRate = (1f / attackSpeed) *
                         (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
    }

    void FollowFormation()
    {
        float   side      = (index % 2 == 0) ? -1f : 1f;
        float   offset    = Mathf.Ceil((index + 1) / 2f) * spacing;
        Vector3 right     = playerTransform.right;
        Vector3 targetPos = playerTransform.position + right * side * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * 10f
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            playerTransform.rotation,
            Time.deltaTime * 10f
        );
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3    dir      = playerTransform.forward;
        Vector3    spawnPos = transform.position + dir * 1f;
        Quaternion rot      = Quaternion.LookRotation(dir);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);

        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null && playerStats != null)
        {
            // Utilise exactement les mêmes stats que le joueur
            projScript.damage         = playerStats.GetStat(StatType.RangedDamage);
            projScript.speed          = playerStats.GetStat(StatType.ProjectileSpeed);
            projScript.lifeStealRatio = playerStats.GetStat(StatType.LifeSteal);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}