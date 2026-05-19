// Scripts/VFX/RegenerationVFX.cs
using UnityEngine;
using System.Collections.Generic;

public class RegenerationVFX : MonoBehaviour
{
    [Header("Particules")]
    public ParticleSystem healParticles;

    [Header("Surbrillance")]
    public Color  glowColor      = new Color(0f, 1f, 0.3f, 0.3f);
    public float  pulseSpeed     = 2f;
    public float  pulseAmplitude = 0.08f;

    private List<Material> originalMaterials = new List<Material>();
    private List<Material> glowMaterials     = new List<Material>();
    private List<Renderer> renderers         = new List<Renderer>();
    private float          elapsed;

    void Awake()
    {
        SetupGlow();

        if (healParticles != null)
        {
            var main        = healParticles.main;
            main.startColor = new Color(0f, 0.9f, 0.3f, 0.8f);
            healParticles.Play();
        }
    }

    void SetupGlow()
    {
        Renderer[] rends = GetComponentsInParent<Renderer>();
        foreach (Renderer r in rends)
        {
            renderers.Add(r);
            originalMaterials.Add(r.material);

            // Crée un material de surbrillance verte
            Material glowMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (glowMat != null)
            {
                glowMat.SetFloat("_Surface",  1f);
                glowMat.SetFloat("_Blend",    0f);
                glowMat.SetFloat("_ZWrite",   0f);
                glowMat.SetInt("_SrcBlend",
                    (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                glowMat.SetInt("_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                glowMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                glowMat.renderQueue = 3000;
                glowMat.SetColor("_BaseColor",    glowColor);
                glowMat.SetColor("_EmissionColor", new Color(0f, 0.8f, 0.2f, 1f));
                glowMat.EnableKeyword("_EMISSION");
                glowMaterials.Add(glowMat);
            }
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        float pulse = glowColor.a +
                      Mathf.Sin(elapsed * pulseSpeed) * pulseAmplitude;
        pulse = Mathf.Clamp(pulse, 0.1f, 0.6f);

        // Pulse la surbrillance sur chaque renderer
        for (int i = 0; i < glowMaterials.Count; i++)
        {
            if (glowMaterials[i] == null) continue;
            Color c = glowMaterials[i].GetColor("_BaseColor");
            c.a     = pulse;
            glowMaterials[i].SetColor("_BaseColor", c);

            if (i < renderers.Count && renderers[i] != null)
                renderers[i].material = glowMaterials[i];
        }
    }

    void OnDestroy()
    {
        // Remet les materials originaux
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null && i < originalMaterials.Count)
                renderers[i].material = originalMaterials[i];
        }

        if (healParticles != null)
            healParticles.Stop();
    }
}