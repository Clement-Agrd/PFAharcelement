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
    public VFXPlayer warningVFX;
    public VFXPlayer damageVFX;
    public GameObject piranaPrefab;

    private void Start()
    {
        StartCoroutine(ZoneRoutine());
    }


    private IEnumerator ZoneRoutine()
    {
        warningVFX?.Play();
        yield return new WaitForSeconds(warningDuration);

        warningVFX?.Stop();
        damageVFX?.Play();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);

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