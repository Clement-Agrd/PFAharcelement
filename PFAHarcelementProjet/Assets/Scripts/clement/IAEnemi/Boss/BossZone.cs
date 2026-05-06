using System.Collections;
using UnityEngine;

public class BossZone : MonoBehaviour
{
    [Header("Timing")]
    public float warningDuration = 1.8f; // temps pendant lequel la zone est visible
    public float damageDuration  = 0.4f; // durée du flash de dégâts

    [Header("Stats")]
    public float damage     = 25f;
    public float radius     = 2.5f;
    public int   piranaCount = 3;

    [Header("Refs")]
    public GameObject piranaPrefab;      // prefab pirana qui monte vers le haut
    public ParticleSystem warningVFX;    // cercle au sol qui pulse
    public ParticleSystem damageVFX;     // explosion de piranhas

    private void Start()
    {
        StartCoroutine(ZoneRoutine());
    }

    private IEnumerator ZoneRoutine()
    {
        // Phase warning
        warningVFX?.Play();
        yield return new WaitForSeconds(warningDuration);

        // Phase dégâts
        warningVFX?.Stop();
        damageVFX?.Play();

        // Dégâts sur le joueur
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

        // Spawn piranhas qui montent
        for (int i = 0; i < piranaCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * (radius * 0.5f);
            offset.y = 0f;
            Instantiate(piranaPrefab, transform.position + offset, Quaternion.identity);
        }

        yield return new WaitForSeconds(damageDuration);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}