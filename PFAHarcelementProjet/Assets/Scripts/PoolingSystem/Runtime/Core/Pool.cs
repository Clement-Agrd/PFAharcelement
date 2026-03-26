using UnityEngine;

public class PoolTest2D : MonoBehaviour
{
    public string poolID = "TestSprite";
    public float spawnInterval = 0.5f;
    public float despawnTime = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);
    }

    void Spawn()
    {
        GameObject obj = PoolManager.Instance.Spawn(poolID, RandomPosition(), Quaternion.identity);
        if (obj != null)
        {
            // Désactive automatiquement après despawnTime
            StartCoroutine(ReturnAfterTime(obj, despawnTime));
        }
    }

    System.Collections.IEnumerator ReturnAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.GetComponent<PoolMember>().ReturnToPool();
    }

    Vector3 RandomPosition()
    {
        float x = Random.Range(-8f, 8f);
        float y = Random.Range(-4f, 4f);
        return new Vector3(x, y, 0);
    }
}