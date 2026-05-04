using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.4f;

    Coroutine flashRoutine;

    void Awake()
    {
        flashImage.color = new Color(1, 0, 0, 0);
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        float t = 0f;

        while (t < flashDuration)
        {
            float alpha = Mathf.Lerp(maxAlpha, 0, t / flashDuration);
            flashImage.color = new Color(1, 0, 0, alpha);

            t += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 0, 0, 0);
    }
}