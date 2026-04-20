
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBaseData", menuName = "Roguelike/Player Base Data")]
public class PlayerBaseData : ScriptableObject
{
    [Header("Survie")]
    public float hp                 = 100f;
    public float tankiness          = 0f;    // % de réduction de dégâts (0 à 1)
    public float lifeSteal          = 0f;    // % de vol de vie (0 à 1)

    [Header("Combat")]
    public float damage             = 10f;
    public float attackSpeed        = 4f;    // attaques par seconde
    public float projectileSpeed    = 15f;
    public float cooldownReduction  = 0f;    // % de réduction (0 à 1)

    [Header("Déplacement")]
    public float moveSpeed          = 6f;
}