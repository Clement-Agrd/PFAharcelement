// Scripts/VFX/MirrorVFX.cs
using UnityEngine;

public class MirrorVFX : MonoBehaviour
{
    [Header("Aura")]
    public ParticleSystem auraParticles;
    public MeshRenderer   playerMesh;

    [Header("Paramètres")]
    public float pulseSpeed     = 3f;
    public float pulseAmplitude = 0.06f;
    public Color auraColor      = new Color(1f, 0.5f, 0f, 0.35f);

    private Material auraMat;
    private float    elapsed;

    void Awake()
    {
        SetupMaterial();

        if (auraParticles != null)
        {
            var main        = auraParticles.main;
            main.startColor = auraColor;
            auraParticles.Play();
        }
    }

    void SetupMaterial()
    {
        if (playerMesh == null)
        {
            playerMesh = GetComponentInParent<MeshRenderer>();
            if (playerMesh == null)
                playerMesh = GetComponentInChildren<MeshRenderer>();
        }

        if (playerMesh == null) return;

        auraMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (auraMat == null) return;

        auraMat.SetFloat("_Surface",  1f);
        auraMat.SetFloat("_Blend",    0f);
        auraMat.SetFloat("_ZWrite",   0f);
        auraMat.SetInt("_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        auraMat.SetInt("_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        auraMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        auraMat.renderQueue = 3000;
        auraMat.SetColor("_BaseColor",    auraColor);
        auraMat.SetColor("_EmissionColor", new Color(1f, 0.4f, 0f, 1f));
        auraMat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        float pulse = 0.3f + Mathf.Sin(elapsed * pulseSpeed) * pulseAmplitude;

        if (auraMat != null)
        {
            Color c = auraMat.GetColor("_BaseColor");
            c.a     = pulse;
            auraMat.SetColor("_BaseColor", c);
        }
    }

    void OnDestroy()
    {
        if (auraParticles != null)
            auraParticles.Stop();
    }
}