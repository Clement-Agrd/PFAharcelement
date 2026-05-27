// Scripts/VFX/ShieldCollider.cs
using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    private ShieldUltimate shieldUltimate;
    private bool           isActive = false;

    void Awake()
    {
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

        if (other.CompareTag("EnemyProjectile"))
        {
            // ← Son à chaque impact
            if (UltimateSoundManager.Instance != null)
                UltimateSoundManager.Instance.PlayShieldHit();

            if (shieldUltimate != null)
                shieldUltimate.NotifyHit(other.transform.position);

            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDir = (other.transform.position -
                                   transform.position).normalized;
                rb.AddForce(pushDir * 10f, ForceMode.Impulse);
            }
        }
    }
}