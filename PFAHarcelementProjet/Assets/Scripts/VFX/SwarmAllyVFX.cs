// Scripts/VFX/SwarmAllyVFX.cs
using UnityEngine;

public class SwarmAllyVFX : MonoBehaviour
{
    [Header("Aura")]
    public ParticleSystem auraParticles;
    public Color          auraColor = new Color(0f, 0.5f, 1f, 1f);

    [Header("Taille aléatoire")]
    public float minScale = 0.4f;
    public float maxScale = 1.2f;

    private Material auraMat;
    private float    elapsed;
    private float    pulseSpeed = 3f;

    void Start()
    {
        // Taille aléatoire
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * scale;

        SetupAura();
    }

    void SetupAura()
    {
        if (auraParticles == null) return;

        var main = auraParticles.main;
        main.startColor = auraColor;
        main.startSize  = 0.3f * transform.localScale.x;
        auraParticles.Play();
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Pulse de scale doux
        float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * 0.05f;
        transform.localScale = transform.localScale.normalized *
                               transform.localScale.magnitude * pulse;
    }
}