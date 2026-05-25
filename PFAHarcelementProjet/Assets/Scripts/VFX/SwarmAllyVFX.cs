// Scripts/VFX/SwarmAllyVFX.cs
using UnityEngine;

public class SwarmAllyVFX : MonoBehaviour
{
    [Header("Aura")]
    public ParticleSystem auraParticles;
    public Color          auraColor = new Color(0f, 0.5f, 1f, 1f);

    [Header("Taille aléatoire")]
    public float minScale = 0.5f;
    public float maxScale = 0.9f;

    private float elapsed;
    private float pulseSpeed = 3f;
    private float baseScale;

    void Start()
    {
        // Taille aléatoire mais plus petite que le joueur
        baseScale            = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * baseScale;

        SetupAura();
    }

    void SetupAura()
    {
        if (auraParticles == null) return;

        var main        = auraParticles.main;
        main.startColor = auraColor;
        main.startSize  = 0.2f * baseScale;
        auraParticles.Play();
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Pulse de scale doux
        float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * 0.04f;
        transform.localScale = Vector3.one * baseScale * pulse;
    }
}