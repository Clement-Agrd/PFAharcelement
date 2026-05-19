// Scripts/VFX/ShockwaveVFX.cs
using UnityEngine;

public class ShockwaveVFX : MonoBehaviour
{
    [Header("Onde")]
    public float maxRadius    = 10f;
    public float expandSpeed  = 15f;
    public float duration     = 0.8f;

    private Material mat;
    private float    elapsed;
    private bool     finished = false;

    void Awake()
    {
        // Crée un disque qui s'expanse
        MeshRenderer rend = GetComponent<MeshRenderer>();
        if (rend != null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (mat != null)
            {
                mat.SetFloat("_Surface",  1f);
                mat.SetFloat("_Blend",    0f);
                mat.SetFloat("_ZWrite",   0f);
                mat.SetInt("_SrcBlend",
                    (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend",
                    (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
                mat.SetColor("_BaseColor",
                    new Color(0.8f, 0.95f, 1f, 0.6f));
                mat.SetColor("_EmissionColor",
                    new Color(0.5f, 0.8f, 1f, 1f));
                mat.EnableKeyword("_EMISSION");
                rend.material = mat;
            }
        }

        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (finished) return;

        elapsed += Time.deltaTime;
        float progress = elapsed / duration;

        // Expanse le disque
        float currentRadius = Mathf.Lerp(0f, maxRadius, progress);
        transform.localScale = new Vector3(
            currentRadius,
            0.05f,
            currentRadius
        );

        // Fade out
        if (mat != null)
        {
            float alpha = Mathf.Lerp(0.6f, 0f, progress);
            Color c     = mat.GetColor("_BaseColor");
            c.a         = alpha;
            mat.SetColor("_BaseColor", c);
        }

        if (progress >= 1f)
        {
            finished = true;
            Destroy(gameObject);
        }
    }
}