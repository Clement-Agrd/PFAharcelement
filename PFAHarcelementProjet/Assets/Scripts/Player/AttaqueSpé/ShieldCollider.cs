// Scripts/VFX/ShieldCollider.cs
using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    private ShieldUltimate shieldUltimate;
    private bool           isActive = false;

    void Awake()
    {
        // Remonte jusqu'au joueur pour trouver ShieldUltimate
        shieldUltimate = GetComponentInParent<ShieldUltimate>();

        if (shieldUltimate == null)
            Debug.LogWarning("⚠️ ShieldCollider : ShieldUltimate introuvable");
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Bloque les projectiles ennemis
        if (other.CompareTag("EnemyProjectile"))
        {
            // Effet de hit sur le bouclier
            if (shieldUltimate != null)
                shieldUltimate.NotifyHit(other.transform.position);

            Debug.Log("🛡️ Projectile bloqué par le bouclier");
            Destroy(other.gameObject);
            return;
        }

        // Bloque les ennemis en mêlée
        if (other.CompareTag("Enemy"))
        {
            // Repousse l'ennemi
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(pushDir * 10f, ForceMode.Impulse);
            }
        }
    }
}