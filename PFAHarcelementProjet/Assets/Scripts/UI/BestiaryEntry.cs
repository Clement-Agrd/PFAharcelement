using UnityEngine;

[CreateAssetMenu(fileName = "BestiaryEntry", menuName = "Bestiary/Entry")]
public class BestiaryEntry : ScriptableObject
{
    public string creatureName;
    [TextArea] public string description;
    public Sprite image;
    public int id;
}