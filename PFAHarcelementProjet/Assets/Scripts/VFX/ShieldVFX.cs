// Scripts/VFX/ShieldVFX.cs
using UnityEngine;

public class ShieldVFX : MonoBehaviour
{
    [Header("Références")]
    public MeshRenderer   shieldSphere;
    public ParticleSystem rippleParticles;
    public ParticleSystem edgeParticles;

    [Header("Forme ellipsoïde")]
    public Vector3 baseScale      = new Vector3(4f, 2.5f, 6f);
    public float   pulseSpeed     = 2f;
    public float   pulseAmplitude = 0.05f;

    [Header("Couleur")]
    public Color shieldColor   = new Color(0f, 0.6f, 1f, 0.25f);
    public Color emissionColor = new Color(0f, 0.5f, 1f, 1f);

    private Material shieldMat;
    private float    elapsed;

    void Awake()
    {
        SetupMaterial();

        if (shieldSphere != null)
            shieldSphere.transform.localScale = baseScale;
    }

    void SetupMaterial()
    {
        if (shieldSphere == null) return;

        // Shaders URP dans l'ordre de priorité
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
            shader = Shader.Find("Unlit/Color");

        if (shader == null)
        {
            Debug.LogError("❌ Aucun shader URP trouvé");
            return;
        }

        shieldMat = new Material(shader);

        // Configure la transparence URP
        shieldMat.SetFloat("_Surface",  1f); // 0 = Opaque, 1 = Transparent
        shieldMat.SetFloat("_Blend",    0f); // 0 = Alpha
        shieldMat.SetFloat("_ZWrite",   0f);
        shieldMat.SetFloat("_AlphaClip", 0f);

        shieldMat.SetInt("_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        shieldMat.SetInt("_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        shieldMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        shieldMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");

        shieldMat.renderQueue = 3000;

        // Couleur de base
        shieldMat.SetColor("_BaseColor",      shieldColor);
        shieldMat.SetColor("_EmissionColor",  emissionColor);
        shieldMat.EnableKeyword("_EMISSION");

        shieldSphere.material = shieldMat;

        Debug.Log($"✅ ShieldVFX URP shader : {shader.name}");
    }

    void Update()
    {
        if (shieldSphere == null || shieldMat == null) return;

        elapsed += Time.deltaTime;

        // Pulse de scale
        float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * pulseAmplitude;
        shieldSphere.transform.localScale = baseScale * pulse;

        // Pulse d'opacité
        float alpha = shieldColor.a +
                      Mathf.Sin(elapsed * pulseSpeed * 1.3f) * 0.07f;
        alpha       = Mathf.Clamp(alpha, 0.1f, 0.5f);

        Color c     = shieldMat.GetColor("_BaseColor");
        c.a         = alpha;
        shieldMat.SetColor("_BaseColor", c);
    }

    public void OnHit(Vector3 hitPoint)
    {
        if (rippleParticles == null) return;
        rippleParticles.transform.position = hitPoint;
        rippleParticles.Emit(15);
    }
}