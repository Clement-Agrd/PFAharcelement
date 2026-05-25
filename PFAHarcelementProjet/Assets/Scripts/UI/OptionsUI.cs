using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [Header("Audio")]
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSFX;

    [Header("Affichage")]
    public Slider         sliderBrightness;
    public TMP_Dropdown   dropdownResolution;
    public TMP_Dropdown   dropdownQuality;
    public Toggle         toggleFullscreen;

    [Header("Gameplay")]
    public Slider sliderMouseSensivity;

    [Header("Accessibilité")]
    public TMP_Dropdown dropdownTextSize;
    public TMP_Dropdown dropdownColorblind;
    
    [Header("Boutons")]
    public Button buttonApply;
    public Button buttonBack;

    void OnEnable()
    {
        PopulateDropdowns();
        LoadCurrentValues();
        BindButtons();
    }

    //  Initialisation 

    void PopulateDropdowns()
    {
        // Résolutions disponibles sur la machine
        dropdownResolution.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        foreach (Resolution res in Screen.resolutions)
            options.Add($"{res.width} x {res.height} @ {res.refreshRate}Hz");
        dropdownResolution.AddOptions(options);

        // Qualité graphique depuis Unity Quality Settings
        dropdownQuality.ClearOptions();
        dropdownQuality.AddOptions(
            new System.Collections.Generic.List<string>(QualitySettings.names)
        );

        // Taille du texte
        dropdownTextSize.ClearOptions();
        dropdownTextSize.AddOptions(new System.Collections.Generic.List<string>
            { "Petit", "Normal", "Grand" });

        // Daltonisme
        dropdownColorblind.ClearOptions();
        dropdownColorblind.AddOptions(new System.Collections.Generic.List<string>
            { "Normal", "Deutéranopie", "Protanopie" });
    }

    void LoadCurrentValues()
    {
        OptionsData d = OptionsManager.Instance.GetData();

        sliderMaster         .value = d.masterVolume;
        sliderMusic          .value = d.musicVolume;
        sliderSFX            .value = d.sfxVolume;
        sliderBrightness     .value = d.brightness;
        sliderMouseSensivity .value = d.mouseSensivity;
        dropdownResolution   .value = d.resolutionIndex;
        dropdownQuality      .value = d.qualityIndex;
        toggleFullscreen     .isOn  = d.fullscreen;
        dropdownTextSize     .value = d.textSize;
        dropdownColorblind   .value = d.colorblindMode;

        
    }

    void BindButtons()
    {
        // Sliders
        sliderMaster        .onValueChanged.RemoveAllListeners();
        sliderMusic         .onValueChanged.RemoveAllListeners();
        sliderSFX           .onValueChanged.RemoveAllListeners();
        sliderBrightness    .onValueChanged.RemoveAllListeners();
        sliderMouseSensivity.onValueChanged.RemoveAllListeners();

        sliderMaster        .onValueChanged.AddListener(OptionsManager.Instance.SetMasterVolume);
        sliderMusic         .onValueChanged.AddListener(OptionsManager.Instance.SetMusicVolume);
        sliderSFX           .onValueChanged.AddListener(OptionsManager.Instance.SetSFXVolume);
        sliderBrightness    .onValueChanged.AddListener(OptionsManager.Instance.SetBrightness);
        sliderMouseSensivity.onValueChanged.AddListener(OptionsManager.Instance.SetMouseSensivity);

        // Dropdowns
        dropdownResolution .onValueChanged.RemoveAllListeners();
        dropdownQuality    .onValueChanged.RemoveAllListeners();
        dropdownTextSize   .onValueChanged.RemoveAllListeners();
        dropdownColorblind .onValueChanged.RemoveAllListeners();

        dropdownResolution .onValueChanged.AddListener(OptionsManager.Instance.SetResolution);
        dropdownQuality    .onValueChanged.AddListener(OptionsManager.Instance.SetQuality);
        dropdownTextSize   .onValueChanged.AddListener(OptionsManager.Instance.SetTextSize);
        dropdownColorblind .onValueChanged.AddListener(OptionsManager.Instance.SetColorblindMode);

        // Toggle
        toggleFullscreen.onValueChanged.RemoveAllListeners();
        toggleFullscreen.onValueChanged.AddListener(OptionsManager.Instance.SetFullscreen);
        
        // Appliquer / Retour
        buttonApply.onClick.RemoveAllListeners();
        buttonApply.onClick.AddListener(OptionsManager.Instance.ApplyAndSave);
    }
}