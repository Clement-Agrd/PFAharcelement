using System.Collections;
using UnityEngine;

public class TentacleZone : MonoBehaviour
{
    [Header("Timing")]
    public float warningDuration = 1.2f;
    public float riseSpeed       = 6f;
    public float holdDuration    = 0.4f;
    public float retractSpeed    = 4f;

    [Header("Stats")]
    public float damage = 20f;
    public float radius = 1f;

    [Header("Refs")]
    public ParticleSystem warningVFX;

    private Vector3 hiddenPos;
    private Vector3 targetPos;

    private ParticleSystem[] allVFX;

    void Awake()
    {
        allVFX = GetComponentsInChildren<ParticleSystem>();

        // Le tentacule commence sous le sol
        hiddenPos = transform.position + Vector3.down * 3f;
        targetPos = transform.position;
        transform.position = hiddenPos;
    }

    void Start() => StartCoroutine(TentacleRoutine());

    private IEnumerator TentacleRoutine()
    {
        // Warning — VFX au sol
        foreach (var ps in allVFX) ps.Play();
        yield return new WaitForSeconds(warningDuration);

        // Montée rapide
        yield return StartCoroutine(Move(hiddenPos, targetPos, riseSpeed));

        // Dégâts au contact
        Collider[] hits = Physics.OverlapSphere(targetPos, radius);
        foreach (var hit in hits)
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);

        // Maintien
        yield return new WaitForSeconds(holdDuration);

        // Rétractation
        yield return StartCoroutine(Move(targetPos, hiddenPos, retractSpeed));

        foreach (var ps in allVFX) ps.Stop();
        Destroy(gameObject);
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float speed)
    {
        float t = 0f;
        float duration = Vector3.Distance(from, to) / speed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}