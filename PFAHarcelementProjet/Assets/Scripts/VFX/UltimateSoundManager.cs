// Scripts/Audio/UltimateSoundManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class UltimateSoundManager : MonoBehaviour
{
    public static UltimateSoundManager Instance { get; private set; }

    [Header("Références")]
    public AudioSource        sfxSource;
    public AudioSource        loopSource;
    public AudioMixer         audioMixer;
    public UltimateSoundData  soundData;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Sons one-shot ────────────────────────────────────────────────────────

    public void PlaySwarmLaunch()
    {
        Play(soundData.swarmLaunch);
    }

    public void PlaySpecialLaunch()
    {
        Play(soundData.specialLaunch);
    }

    public void PlayInvisibilityStart()
    {
        Play(soundData.invisibilityStart);
    }

    public void PlayMirrorStart()
    {
        Play(soundData.mirrorStart);
    }

    public void PlayMirrorReflect()
    {
        Play(soundData.mirrorReflect);
    }

    public void PlayShieldStart()
    {
        Play(soundData.shieldStart);
    }

    public void PlayShieldHit()
    {
        Play(soundData.shieldHit);
    }

    public void PlayStatBoostStart()
    {
        Play(soundData.statBoostStart);
    }

    public void PlayShockwaveStart()
    {
        Play(soundData.shockwaveStart);
    }

    // ─── Sons en boucle ───────────────────────────────────────────────────────

    public void PlayRegenLoop()
    {
        if (soundData.regenLoop == null || loopSource == null) return;

        loopSource.clip   = soundData.regenLoop;
        loopSource.loop   = true;
        loopSource.volume = 1f;
        loopSource.Play();
    }

    public void StopRegenLoop()
    {
        if (loopSource == null) return;
        loopSource.Stop();
    }

    // ─── Utilitaire ──────────────────────────────────────────────────────────

    void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}