// Scripts/Audio/MusicManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioMixer  audioMixer;

    [Header("Musiques")]
    public AudioClip       menuMusic;
    public List<AudioClip> gameMusics = new List<AudioClip>();

    [Header("Fondu")]
    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;
    private Coroutine playlistCoroutine;
    private int       currentTrackIndex = 0;
    private bool      isInGame          = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMenuMusic();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public void PlayMenuMusic()
    {
        isInGame = false;

        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }

        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        isInGame = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToGamePlaylist());
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    // ─── Lecture simple ───────────────────────────────────────────────────────

    void PlayMusic(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.clip   = clip;
        audioSource.loop   = true;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    // ─── Playlist en jeu ─────────────────────────────────────────────────────

    IEnumerator FadeToGamePlaylist()
    {
        // Fade out de la musique actuelle
        yield return StartCoroutine(FadeOut());

        // Lance la playlist
        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        playlistCoroutine = StartCoroutine(PlaylistCoroutine());
    }

    IEnumerator PlaylistCoroutine()
    {
        if (gameMusics == null || gameMusics.Count == 0)
        {
            Debug.LogWarning("⚠️ MusicManager : aucune musique en jeu");
            yield break;
        }

        currentTrackIndex = 0;

        while (isInGame)
        {
            AudioClip clip = gameMusics[currentTrackIndex];

            if (clip == null)
            {
                NextTrack();
                continue;
            }

            // Fade in de la nouvelle piste
            audioSource.clip   = clip;
            audioSource.loop   = false;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeIn());

            Debug.Log($"🎵 Musique en jeu : {clip.name}");

            // Attend la fin de la piste
            while (audioSource.isPlaying && isInGame)
                yield return null;

            if (!isInGame) break;

            // Fade out avant la prochaine piste
            yield return StartCoroutine(FadeOut());

            NextTrack();
        }
    }

    void NextTrack()
    {
        if (gameMusics.Count == 0) return;
        currentTrackIndex = (currentTrackIndex + 1) % gameMusics.Count;
    }

    // ─── Fondus ──────────────────────────────────────────────────────────────

    IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsed     = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed            += Time.unscaledDeltaTime;
            audioSource.volume  = Mathf.Lerp(startVolume, 0f,
                                  elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        audioSource.volume = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed            += Time.unscaledDeltaTime;
            audioSource.volume  = Mathf.Lerp(0f, 1f,
                                  elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
    }
}