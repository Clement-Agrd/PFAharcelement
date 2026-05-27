// Scripts/Audio/PlayerSoundManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance { get; private set; }

    [Header("Références")]
    public AudioSource      sfxSource;
    public AudioMixer       audioMixer;
    public PlayerSoundData  soundData;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayShoot()  => Play(soundData?.shoot);
    public void PlayMelee()  => Play(soundData?.melee);
    public void PlayDash()   => Play(soundData?.dash);

    void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}