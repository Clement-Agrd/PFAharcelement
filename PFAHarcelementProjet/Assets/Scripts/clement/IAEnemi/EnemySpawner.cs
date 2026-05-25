using UnityEngine;

public class EnemySpawner : MonoBehaviour, IRoomEnter
{
    public GameObject enemyPrefab;

    public void OnRoomEnter()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // ✅ On notifie que des ennemis existent
        CombatRoomEndTrigger trigger = FindObjectOfType<CombatRoomEndTrigger>();

        if (trigger != null)
        {
            trigger.NotifyEnemiesSpawned();
        }
    }
}
