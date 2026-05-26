// Scripts/Audio/AmbientManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class AmbientManager : MonoBehaviour
{
    public static AmbientManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource ambientSource;
    public AudioMixer  audioMixer;

    [Header("Son ambiant")]
    public AudioClip ambientClip;

    [Header("Fondu")]
    public float fadeDuration = 1.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAmbient()
    {
        if (ambientClip == null || ambientSource == null) return;

        ambientSource.clip   = ambientClip;
        ambientSource.loop   = true;
        ambientSource.volume = 0f;
        ambientSource.Play();

        StartCoroutine(FadeIn());
    }

    public void StopAmbient()
    {
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed            += Time.unscaledDeltaTime;
            ambientSource.volume = Mathf.Lerp(0f, 1f,
                elapsed / fadeDuration);
            yield return null;
        }

        ambientSource.volume = 1f;
    }

    System.Collections.IEnumerator FadeOut()
    {
        float startVolume = ambientSource.volume;
        float elapsed     = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed            += Time.unscaledDeltaTime;
            ambientSource.volume = Mathf.Lerp(startVolume, 0f,
                elapsed / fadeDuration);
            yield return null;
        }

        ambientSource.volume = 0f;
        ambientSource.Stop();
    }
}