// Scripts/VFX/StatBoostVFX.cs
using UnityEngine;

public class StatBoostVFX : MonoBehaviour
{
    [Header("Particules")]
    public ParticleSystem auraParticles;    // aura autour du requin
    public ParticleSystem trailParticles;   // traînée derrière
    public ParticleSystem burstParticles;   // burst au déclenchement

    [Header("Paramètres")]
    public float auraRadius    = 2f;
    public float trailLifetime = 0.3f;

    private bool isActive = false;

    public void Activate()
    {
        if (isActive) return;
        isActive = true;

        if (auraParticles  != null) auraParticles.Play();
        if (trailParticles != null) trailParticles.Play();

        // Burst initial
        if (burstParticles != null)
        {
            burstParticles.Stop();
            burstParticles.Play();
        }

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        if (!isActive) return;
        isActive = false;

        if (auraParticles  != null) auraParticles.Stop();
        if (trailParticles != null) trailParticles.Stop();

        // Laisse les particules existantes finir
        Invoke(nameof(HideAfterDelay), 1f);
    }

    void HideAfterDelay()
    {
        gameObject.SetActive(false);
    }
}