using UnityEngine;

public class TestBuff : MonoBehaviour
{
    public PlayerStats playerStats;

    void Update()
    {
        // Appuie sur T pour ajouter un buff
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerStats.AddModifier(new StatModifier(StatType.MoveSpeed, ModifierType.Percent, 0.5f));
            playerStats.AddModifier(new StatModifier(StatType.Damage,    ModifierType.Flat,    10f));
            Debug.Log("Buff appliqué !");
            Debug.Log($"MoveSpeed finale : {playerStats.GetStat(StatType.MoveSpeed)}");
            Debug.Log($"Damage finale : {playerStats.GetStat(StatType.Damage)}");
        }
    }
}