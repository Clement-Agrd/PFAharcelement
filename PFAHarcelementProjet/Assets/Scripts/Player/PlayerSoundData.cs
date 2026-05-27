// Scripts/Audio/PlayerSoundData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundData",
    menuName = "Roguelike/Player Sound Data")]
public class PlayerSoundData : ScriptableObject
{
    [Header("Combat")]
    public AudioClip shoot;
    public AudioClip melee;
    public AudioClip dash;
}