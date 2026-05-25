using UnityEngine;

public class CombatRoomEndTrigger : MonoBehaviour
{
    private bool triggered = false;
    private bool hasSpawnedEnemies = false;

    public void NotifyEnemiesSpawned()
    {
        hasSpawnedEnemies = true;
    }

    void Update()
    {
        if (triggered) return;
        if (!hasSpawnedEnemies) return;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            triggered = true;
            StageManager.Instance.OnRoomEnd();
        }
    }
}