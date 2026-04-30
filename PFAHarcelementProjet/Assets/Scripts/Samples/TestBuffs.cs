// Scripts/Tests/TestBuff.cs
using UnityEngine;

public class TestBuff : MonoBehaviour
{
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("❌ PlayerStats introuvable sur " + gameObject.name);
            return;
        }

        Debug.Log("=== STATS DE BASE ===");
        Debug.Log($"HP              : {playerStats.GetStat(StatType.MaxHealth)}");
        Debug.Log($"MoveSpeed       : {playerStats.GetStat(StatType.MoveSpeed)}");
        Debug.Log($"MeleeDamage     : {playerStats.GetStat(StatType.MeleeDamage)}");
        Debug.Log($"RangedDamage    : {playerStats.GetStat(StatType.RangedDamage)}");
        Debug.Log($"AttackSpeed     : {playerStats.GetStat(StatType.AttackSpeed)}");
        Debug.Log($"ProjectileSpeed : {playerStats.GetStat(StatType.ProjectileSpeed)}");
        Debug.Log($"Tankiness       : {playerStats.GetStat(StatType.Tankiness)}");
        Debug.Log($"LifeSteal       : {playerStats.GetStat(StatType.LifeSteal)}");
        Debug.Log($"CooldownRed.    : {playerStats.GetStat(StatType.CooldownReduction)}");
    }

    void Update()
    {
        if (playerStats == null) return;

        // T : buff mêlée
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerStats.AddModifier(new StatModifier(StatType.MeleeDamage, ModifierType.Flat, 15f));
            Debug.Log("✅ [T] Buff MeleeDamage +15 appliqué");
            Debug.Log($"MeleeDamage maintenant : {playerStats.GetStat(StatType.MeleeDamage)}");
        }

        // Y : buff distance
        if (Input.GetKeyDown(KeyCode.Y))
        {
            playerStats.AddModifier(new StatModifier(StatType.RangedDamage, ModifierType.Percent, 0.3f));
            Debug.Log("✅ [Y] Buff RangedDamage +30% appliqué");
            Debug.Log($"RangedDamage maintenant : {playerStats.GetStat(StatType.RangedDamage)}");
        }

        // U : buff vitesse de déplacement
        if (Input.GetKeyDown(KeyCode.U))
        {
            playerStats.AddModifier(new StatModifier(StatType.MoveSpeed, ModifierType.Percent, 0.5f));
            Debug.Log("✅ [U] Buff MoveSpeed +50% appliqué");
            Debug.Log($"MoveSpeed maintenant : {playerStats.GetStat(StatType.MoveSpeed)}");
        }

        // I : buff HP
        if (Input.GetKeyDown(KeyCode.I))
        {
            playerStats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.Flat, 50f));
            Debug.Log("✅ [I] Buff HP +50 appliqué");
            Debug.Log($"HP maintenant : {playerStats.GetStat(StatType.MaxHealth)}");
        }

        // R : reset tous les buffs
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerStats.ClearAllModifiers();
            Debug.Log("🔄 [R] Tous les buffs réinitialisés");
            Debug.Log($"MeleeDamage  : {playerStats.GetStat(StatType.MeleeDamage)}");
            Debug.Log($"RangedDamage : {playerStats.GetStat(StatType.RangedDamage)}");
            Debug.Log($"MoveSpeed    : {playerStats.GetStat(StatType.MoveSpeed)}");
            Debug.Log($"HP           : {playerStats.GetStat(StatType.MaxHealth)}");
        }
    }
}