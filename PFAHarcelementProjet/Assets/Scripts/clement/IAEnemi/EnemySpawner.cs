using UnityEngine;

public class EnemySpawner : MonoBehaviour, IRoomEnter
{
    public GameObject enemyPrefab;

    public void OnRoomEnter()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}