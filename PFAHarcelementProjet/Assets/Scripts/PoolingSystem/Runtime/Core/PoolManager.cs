using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private List<PoolConfig> pools;

    private Dictionary<string, ObjectPool> poolDictionary =
        new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        foreach (PoolConfig config in pools)
        {
            if (config == null || config.prefab == null)
                continue;

            Transform parent = new GameObject("Pool_" + config.poolID).transform;
            parent.SetParent(transform);

            ObjectPool pool = new ObjectPool(
                config.poolID,
                config.prefab,
                config.initialSize,
                config.expandable,
                parent
            );

            poolDictionary.Add(config.poolID, pool);
            PoolTracker.Register(config.poolID, pool);
        }
    }

    public GameObject Spawn(string id, Vector3 position, Quaternion rotation)
    {
        ObjectPool pool;
        if (!poolDictionary.TryGetValue(id, out pool))
        {
            Debug.LogWarning("Pool '" + id + "' not found.");
            return null;
        }

        return pool.Get(position, rotation);
    }

    public T Spawn<T>(string id, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = Spawn(id, position, rotation);
        if (obj == null) return null;

        return obj.GetComponent<T>();
    }

    public void ReturnToPool(PoolMember member)
    {
        if (member == null) return;

        ObjectPool pool;
        if (!poolDictionary.TryGetValue(member.PoolID, out pool))
            return;

        pool.Return(member);
    }
}