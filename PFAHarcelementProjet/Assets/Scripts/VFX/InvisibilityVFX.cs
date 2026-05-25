// Scripts/VFX/InvisibilityVFX.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvisibilityVFX : MonoBehaviour
{
    [Header("Paramètres")]
    public float invisibleAlpha = 0.15f;
    public float fadeSpeed      = 3f;

    [Header("Références directes")]
    public List<Renderer> sharkRenderers = new List<Renderer>();
    public Material       transparentMaterial;

    [Header("Particules")]
    public ParticleSystem disappearParticles;

    private List<Material> originalMaterials = new List<Material>();
    private bool           isFadingIn        = false;

    public void FadeOut()
    {
        SaveOriginalMaterials();

        if (disappearParticles != null)
        {
            disappearParticles.Stop();
            disappearParticles.Play();
        }

        StartCoroutine(FadeCoroutine(1f, invisibleAlpha));
    }

    public void FadeIn()
    {
        isFadingIn = true;
        StartCoroutine(FadeCoroutine(invisibleAlpha, 1f));
    }

    void SaveOriginalMaterials()
    {
        originalMaterials.Clear();

        foreach (Renderer r in sharkRenderers)
        {
            if (r == null) continue;
            originalMaterials.Add(r.material);

            if (transparentMaterial != null)
                r.material = transparentMaterial;
        }

        Debug.Log($"👻 {sharkRenderers.Count} renderers du requin trouvés");
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer r in sharkRenderers)
        {
            if (r == null) continue;
            if (!r.material.HasProperty("_BaseColor")) continue;

            Color c = r.material.GetColor("_BaseColor");
            c.a     = alpha;
            r.material.SetColor("_BaseColor", c);
        }
    }

    IEnumerator FadeCoroutine(float fromAlpha, float toAlpha)
    {
        float elapsed  = 0f;
        float duration = Mathf.Max(0.01f, 1f / fadeSpeed);

        while (elapsed < duration)
        {
            elapsed    += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha,
                          Mathf.Clamp01(elapsed / duration));
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(toAlpha);

        if (isFadingIn)
        {
            for (int i = 0; i < sharkRenderers.Count; i++)
            {
                if (sharkRenderers[i] != null && i < originalMaterials.Count)
                    sharkRenderers[i].material = originalMaterials[i];
            }
            Destroy(gameObject);
        }
    }
}