// Scripts/Audio/PlayerSoundManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance { get; private set; }

    [Header("Références")]
    public AudioMixer      audioMixer;
    public PlayerSoundData soundData;

    [Header("Audio Sources")]
    public AudioSource meleeSource;
    public AudioSource dashSource;

    private bool muted = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Gardé vide pour compatibilité
    public void SetShootPressed(bool pressed) { }

    public void PlayMelee()
    {
        if (muted) return;
        if (meleeSource == null) return;
        if (soundData?.melee == null) return;
        meleeSource.PlayOneShot(soundData.melee);
    }

    public void PlayDash()
    {
        if (muted) return;
        if (dashSource == null) return;
        if (soundData?.dash == null) return;
        dashSource.PlayOneShot(soundData.dash);
    }

    public void Mute()
    {
        muted = true;
        if (meleeSource != null) meleeSource.Stop();
        if (dashSource  != null) dashSource.Stop();
        Debug.Log("🔇 Sons mutés");
    }

    public void Unmute()
    {
        muted = false;
        Debug.Log("🔊 Sons réactivés");
    }
}