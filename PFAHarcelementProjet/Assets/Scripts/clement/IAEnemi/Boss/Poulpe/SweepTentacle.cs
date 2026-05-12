using System.Collections;
using UnityEngine;

public class SweepTentacle : MonoBehaviour
{
    [Header("Timings")]
    public float fallDelay    = 0f;   // décalage entre les tentacules d'une même ligne
    public float fallDuration = 0.3f;
    public float slideDuration = 2f;

    [Header("Slide")]
    public Vector3 slideDirection = Vector3.forward; // direction du glissement
    public float   slideDistance  = 20f;

    [Header("Stats")]
    public float damage = 15f;
    public float hitRadius = 0.8f;

    private bool  isActive  = false;
    private float slideTimer;

    void Start() => StartCoroutine(SweepRoutine());

    private IEnumerator SweepRoutine()
    {
        yield return new WaitForSeconds(fallDelay);

        // Tombe du haut
        Vector3 startPos  = transform.position + Vector3.up * 8f;
        Vector3 landPos   = transform.position;
        transform.position = startPos;

        yield return StartCoroutine(Move(startPos, landPos, fallDuration));

        // Impact — caméra shake léger si tu en as un
        isActive = true;

        // Glisse à travers l'arène
        Vector3 slideTarget = landPos + slideDirection.normalized * slideDistance;
        yield return StartCoroutine(Move(landPos, slideTarget, slideDuration));

        isActive = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (!isActive) return;

        // Dégâts continus pendant le glissement
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hit in hits)
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()
                   ?.TakeDamage(damage * Time.deltaTime);
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(from, to,
                                              Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
        Gizmos.DrawRay(transform.position,
                       slideDirection.normalized * slideDistance);
    }
}