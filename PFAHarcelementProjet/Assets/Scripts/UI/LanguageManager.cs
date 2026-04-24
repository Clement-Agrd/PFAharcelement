using UnityEngine;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyLanguage(string lang)
    {
        // Pour l'instant on met à jour les textes du menu principal
        // Plus tard on branchera le système de localisation complet
        UpdateMenuTexts(lang);
        Debug.Log($"✅ Langue appliquée : {lang}");
    }

    void UpdateMenuTexts(string lang)
    {
        bool fr = lang == "fr";

        SetText("TitleAudio",          fr ? "AUDIO"               : "AUDIO");
        SetText("TitleAffichage",      fr ? "AFFICHAGE"           : "DISPLAY");
        SetText("TitleGameplay",       fr ? "GAMEPLAY"            : "GAMEPLAY");
        SetText("TitleAccessibilite",  fr ? "ACCESSIBILITÉ"       : "ACCESSIBILITY");
        SetText("TitleLangue",         fr ? "LANGUE"              : "LANGUAGE");
        SetText("LabelMaster",         fr ? "Volume Global"       : "Master Volume");
        SetText("LabelMusic",          fr ? "Musique"             : "Music");
        SetText("LabelSFX",            fr ? "Effets Sonores"      : "Sound Effects");
        SetText("LabelBrightness",     fr ? "Luminosité"          : "Brightness");
        SetText("LabelResolution",     fr ? "Résolution"          : "Resolution");
        SetText("LabelQuality",        fr ? "Qualité Graphique"   : "Graphics Quality");
        SetText("LabelFullscreen",     fr ? "Plein Écran"         : "Fullscreen");
        SetText("LabelMouseSensivity", fr ? "Sensibilité Souris"  : "Mouse Sensitivity");
        SetText("LabelTextSize",       fr ? "Taille du Texte"     : "Text Size");
        SetText("LabelColorblind",     fr ? "Mode Daltonisme"     : "Colorblind Mode");
        SetText("ButtonApply",         fr ? "Appliquer"           : "Apply");
        SetText("ButtonBack",          fr ? "Retour"              : "Back");
        SetText("ButtonPlay",          fr ? "Jouer"               : "Play");
        SetText("ButtonOptions",       fr ? "Options"             : "Options");
        SetText("ButtonCredits",       fr ? "Crédits"             : "Credits");
        SetText("ButtonQuit",          fr ? "Quitter"             : "Quit");
        SetText("ButtonStatTree",      fr ? "Arbre de Stats"      : "Stat Tree");
    }

    void SetText(string objectName, string text)
    {
        GameObject go = GameObject.Find(objectName);
        if (go == null) return;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            // Cherche dans les enfants (cas des boutons)
            tmp = go.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (tmp != null)
            tmp.text = text;
    }
}