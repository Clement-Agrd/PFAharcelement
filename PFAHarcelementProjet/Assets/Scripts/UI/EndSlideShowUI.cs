// Scripts/UI/EndSlideshowUI.cs
using UnityEngine;
using UnityEngine.UI;

public class EndSlideshowUI : MonoBehaviour
{
    public static EndSlideshowUI Instance { get; private set; }

    [Header("Slides de fin")]
    public Image    displayImage;
    public Sprite[] slides;
    public float    displayDuration = 3f;
    public float    fadeDuration    = 1f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // Cache l'image mais garde le Canvas actif
        if (displayImage != null)
            displayImage.gameObject.SetActive(false);
    }

    public void Play(System.Action onFinished)
    {
        if (slides == null || slides.Length == 0 || displayImage == null)
        {
            onFinished?.Invoke();
            return;
        }

        displayImage.gameObject.SetActive(true); // Active l'image, pas le Canvas
        StartCoroutine(PlaySlideshow(onFinished));
    }

    System.Collections.IEnumerator PlaySlideshow(System.Action onFinished)
    {
        displayImage.color = new Color(1f, 1f, 1f, 0f);

        foreach (Sprite slide in slides)
        {
            displayImage.sprite = slide;

            yield return StartCoroutine(Fade(0f, 1f));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(Fade(1f, 0f));
        }

        displayImage.gameObject.SetActive(false); // ← ajout
        onFinished?.Invoke();
    }

    System.Collections.IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            displayImage.color = new Color(1f, 1f, 1f,
                Mathf.Lerp(fromAlpha, toAlpha,
                    Mathf.Clamp01(t / fadeDuration)));
            yield return null;
        }

        displayImage.color = new Color(1f, 1f, 1f, toAlpha);
    }
}