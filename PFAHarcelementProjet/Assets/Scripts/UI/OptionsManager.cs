// Scripts/UI/OptionsManager.cs
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [Header("Audio")]
    public AudioMixer audioMixer;

    private OptionsData data     = new OptionsData();
    private string SavePath => Application.persistentDataPath + "/options.json";

    // ─── Unity ───────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
        Apply();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public OptionsData GetData() => data;

    public void SetMasterVolume(float value)   => data.masterVolume    = value;
    public void SetMusicVolume(float value)    => data.musicVolume     = value;
    public void SetSFXVolume(float value)      => data.sfxVolume       = value;
    public void SetBrightness(float value)     => data.brightness      = value;
    public void SetMouseSensivity(float value) => data.mouseSensivity  = value;
    public void SetQuality(int index)          => data.qualityIndex    = index;
    public void SetFullscreen(bool value)      => data.fullscreen      = value;
    public void SetResolution(int index)       => data.resolutionIndex = index;
    public void SetTextSize(int index)         => data.textSize        = index;
    public void SetColorblindMode(int index)   => data.colorblindMode  = index;

    public float GetMouseSensivity() => data.mouseSensivity;

    public void ApplyAndSave()
    {
        Apply();
        Save();
        Debug.Log("✅ Options sauvegardées");
    }

    // ─── Application ─────────────────────────────────────────────────────────

    void Apply()
    {
        // Audio
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", VolumeToDb(data.masterVolume));
            audioMixer.SetFloat("MusicVolume",  VolumeToDb(data.musicVolume));
            audioMixer.SetFloat("SFXVolume",    VolumeToDb(data.sfxVolume));
        }

        // Qualité graphique
        if (data.qualityIndex >= 0 &&
            data.qualityIndex < QualitySettings.names.Length)
            QualitySettings.SetQualityLevel(data.qualityIndex, true);

        // Résolution
        Resolution[] resolutions = Screen.resolutions;
        if (resolutions.Length > 0)
        {
            int safeIndex        = Mathf.Clamp(data.resolutionIndex,
                                               0, resolutions.Length - 1);
            data.resolutionIndex = safeIndex;
            Resolution res       = resolutions[safeIndex];
            Screen.SetResolution(res.width, res.height, data.fullscreen);
        }
        else
            Screen.fullScreen = data.fullscreen;

        // Luminosité via panel UI
        if (BrightnessManager.Instance != null)
            BrightnessManager.Instance.ApplyBrightness(data.brightness);

        // Taille des textes
        if (TextSizeManager.Instance != null)
            TextSizeManager.Instance.ApplyTextSize(data.textSize);

        // Daltonisme
        if (ColorblindManager.Instance != null)
            ColorblindManager.Instance.ApplyColorblindMode(data.colorblindMode);
    }

    // ─── Sauvegarde ──────────────────────────────────────────────────────────

    void Save()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    void Load()
    {
        if (!File.Exists(SavePath)) return;

        try
        {
            data = JsonUtility.FromJson<OptionsData>(File.ReadAllText(SavePath));
        }
        catch
        {
            data = new OptionsData();
            Debug.LogWarning("⚠️ Fichier options corrompu — valeurs par défaut");
        }
    }

    // ─── Utilitaire ──────────────────────────────────────────────────────────

    float VolumeToDb(float value)
    {
        return value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
    }
}