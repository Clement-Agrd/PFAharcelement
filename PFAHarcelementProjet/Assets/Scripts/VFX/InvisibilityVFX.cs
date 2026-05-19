// Scripts/VFX/InvisibilityVFX.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvisibilityVFX : MonoBehaviour
{
    [Header("Paramètres")]
    public float invisibleAlpha  = 0.15f;
    public float fadeSpeed       = 3f;

    [Header("Particules au déclenchement")]
    public ParticleSystem disappearParticles;

    private List<Material>  originalMaterials = new List<Material>();
    private List<Renderer>  renderers         = new List<Renderer>();
    private bool            isFadingIn        = false;
    private bool            isFadingOut       = false;

    public void FadeOut()
    {
        isFadingOut = true;
        isFadingIn  = false;

        // Collecte tous les renderers du joueur
        renderers.Clear();
        originalMaterials.Clear();

        Renderer[] rends = GetComponentsInParent<Renderer>();
        foreach (Renderer r in rends)
        {
            renderers.Add(r);
            originalMaterials.Add(r.material);
        }

        if (disappearParticles != null)
        {
            disappearParticles.Stop();
            disappearParticles.Play();
        }

        StartCoroutine(FadeCoroutine(1f, invisibleAlpha));
    }

    public void FadeIn()
    {
        isFadingIn  = true;
        isFadingOut = false;
        StartCoroutine(FadeCoroutine(invisibleAlpha, 1f));
    }

    IEnumerator FadeCoroutine(float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;
        float duration = 1f / fadeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);

            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
                Color c = r.material.color;
                c.a = alpha;
                r.material.color = c;
            }

            yield return null;
        }

        // Si fade in terminé — remet les materials originaux
        if (isFadingIn)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null && i < originalMaterials.Count)
                    renderers[i].material = originalMaterials[i];
            }
            Destroy(gameObject);
        }
    }
}