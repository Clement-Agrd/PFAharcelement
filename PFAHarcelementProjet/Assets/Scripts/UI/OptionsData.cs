using System;

[Serializable]
public class OptionsData
{
    public float  masterVolume   = 1f;
    public float  musicVolume    = 1f;
    public float  sfxVolume      = 1f;
    public float  brightness     = 1f;
    public float  mouseSensivity = 1f;
    public int    qualityIndex   = 2;
    public bool   fullscreen     = true;
    public int    resolutionIndex = 0;
    public int    textSize       = 1;   // 0 petit, 1 normal, 2 grand
    public int    colorblindMode = 0;   // 0 normal, 1 deuteranopie, 2 protanopie
    public string language       = "fr";
}