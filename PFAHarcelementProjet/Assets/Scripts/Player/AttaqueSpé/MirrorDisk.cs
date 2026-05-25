// Scripts/Ultimates/MirrorDisk.cs
using UnityEngine;

public class MirrorDisk : MonoBehaviour
{
    private PlayerStats playerStats;
    private Transform   playerTransform;

    public float   pushForce = 12f;
    public Vector3 rotOffset = new Vector3(90f, 0f, 0f);

    void Awake()
    {
        Debug.Log("✅ MirrorDisk Awake");
    }

    public void Setup(PlayerStats stats, Transform player)
    {
        playerStats     = stats;
        playerTransform = player;
    }

    void Update()
    {
        if (playerTransform == null) return;

        transform.position = playerTransform.position +
                             playerTransform.forward * 2f;

        transform.rotation = playerTransform.rotation *
                             Quaternion.Euler(rotOffset);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🪞 Miroir touché par : {other.name} tag : {other.tag}");

        if (other.CompareTag("EnemyProjectile"))
        {
            ReflectProjectile(other.gameObject);
            return;
        }

        if (other.CompareTag("Enemy"))
            PushEnemy(other);
    }

    void ReflectProjectile(GameObject proj)
    {
        ProjectileEnemy projScript = proj.GetComponent<ProjectileEnemy>();

        Debug.Log($"🪞 ProjectileEnemy trouvé : {projScript != null}");

        if (projScript == null) return;

        Transform nearestEnemy = FindNearestEnemy();
        Vector3   newDir;

        Debug.Log($"🪞 Ennemi le plus proche : {(nearestEnemy != null ? nearestEnemy.name : "aucun")}");

        if (nearestEnemy != null)
        {
            newDir   = (nearestEnemy.position - proj.transform.position).normalized;
            newDir.y = 0f;
            newDir   = newDir.normalized;
            Debug.Log($"🪞 Projectile renvoyé vers {nearestEnemy.name}");
        }
        else
        {
            newDir = Vector3.Reflect(proj.transform.forward, transform.forward);
            Debug.Log("🪞 Projectile renvoyé — rebond");
        }

        if (playerStats != null)
            projScript.damage += Mathf.RoundToInt(
                playerStats.GetStat(StatType.RangedDamage) * 0.5f);

        projScript.isReflected = true;
        projScript.ResetDirection(newDir);

        Debug.Log($"🪞 isReflected : {projScript.isReflected}");
    }

    void PushEnemy(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 pushDir = (other.transform.position - transform.position).normalized;
        pushDir.y       = 0f;
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);

        Debug.Log($"🪞 Ennemi repoussé : {other.name}");
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies     = GameObject.FindGameObjectsWithTag("Enemy");
        Transform    nearest     = null;
        float        closestDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(
                transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearest     = enemy.transform;
            }
        }

        return nearest;
    }
}