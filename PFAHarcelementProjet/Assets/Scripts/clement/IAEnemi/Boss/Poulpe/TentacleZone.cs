using System.Collections;
using UnityEngine;

public class TentacleZone : MonoBehaviour
{
    int playerHitboxLayer;
    
    [Header("Timing")]
    public float warningDuration = 1.2f;
    public float riseSpeed       = 6f;
    public float holdDuration    = 0.4f;
    public float retractSpeed    = 4f;

    [Header("Stats")]
    public float damage = 20f;
    public float radius = 1f;

    [Header("Refs")]
    public Transform meshRoot; // ← glisse "meshtentaculezone" ici dans l'Inspector

    private Vector3 hiddenPos;
    private Vector3 targetPos;
    private ParticleSystem[] allVFX;

    void Awake()
    {
        playerHitboxLayer = LayerMask.NameToLayer("PlayerHitbox");

        allVFX = GetComponentsInChildren<ParticleSystem>(true);

        targetPos = meshRoot.position;
        hiddenPos = targetPos + Vector3.down * 6f;
        meshRoot.position = hiddenPos;
    }


    void Start() => StartCoroutine(TentacleRoutine());

    private IEnumerator TentacleRoutine()
    {
        // VFX reste au sol pendant le warning
        foreach (var ps in allVFX) ps.Play(true);
        yield return new WaitForSeconds(warningDuration);

        // Le mesh monte
        yield return StartCoroutine(MoveMesh(hiddenPos, targetPos, riseSpeed));

        // Dégâts au contact
        Collider[] hits = Physics.OverlapSphere(targetPos, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == playerHitboxLayer)
            {
                hit.GetComponentInParent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
        
        yield return new WaitForSeconds(holdDuration);

        // Le mesh redescend
        yield return StartCoroutine(MoveMesh(targetPos, hiddenPos, retractSpeed));

        foreach (var ps in allVFX) ps.Stop(true);
        Destroy(gameObject);
    }

    private IEnumerator MoveMesh(Vector3 from, Vector3 to, float speed)
    {
        float t        = 0f;
        float duration = Vector3.Distance(from, to) / speed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            meshRoot.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}