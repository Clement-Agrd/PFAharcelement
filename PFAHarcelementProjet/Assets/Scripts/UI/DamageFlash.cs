using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    public static DamageFlash Instance;

    public Image flashImage;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.4f;

    private Coroutine flashRoutine;

    void Awake()
    {
        Instance = this;

        if (flashImage != null)
            flashImage.color = new Color(1, 0, 0, 0);
    }

    public void Flash()
    {
        if (flashImage == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        float t = 0f;

        while (t < flashDuration)
        {
            float alpha = Mathf.Lerp(maxAlpha, 0f, t / flashDuration);
            flashImage.color = new Color(1, 0, 0, alpha);

            t += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 0, 0, 0);
    }
}