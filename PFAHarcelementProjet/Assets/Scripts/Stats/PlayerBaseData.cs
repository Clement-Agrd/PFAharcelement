// Scripts/Stats/PlayerBaseData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBaseData", menuName = "Roguelike/Player Base Data")]
public class PlayerBaseData : ScriptableObject
{
    [Header("Survie")]
    public float hp                 = 100f;
    public float tankiness          = 0f;
    public float lifeSteal          = 0f;

    [Header("Combat")]
    public float meleeDamage        = 20f;   // ← séparé
    public float rangedDamage       = 10f;   // ← séparé
    public float attackSpeed        = 4f;
    public float projectileSpeed    = 15f;
    public float cooldownReduction  = 0f;

    [Header("Déplacement")]
    public float moveSpeed          = 6f;
}