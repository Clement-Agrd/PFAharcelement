using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly string poolID;
    private readonly GameObject prefab;
    private readonly Queue<GameObject> available;
    private readonly HashSet<GameObject> inUse;
    private readonly Transform parent;
    private readonly bool expandable;

    public ObjectPool(string id, GameObject prefab, int initialSize, bool expandable, Transform parent)
    {
        this.poolID = id;
        this.prefab = prefab;
        this.expandable = expandable;
        this.parent = parent;

        available = new Queue<GameObject>();
        inUse = new HashSet<GameObject>();

        Warmup(initialSize);
    }

    private void Warmup(int amount)
    {
        for (int i = 0; i < amount; i++)
            CreateObject();
    }

    private GameObject CreateObject()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);

        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null)
            member = obj.AddComponent<PoolMember>();

        member.SetPool(poolID);

        available.Enqueue(obj);
        return obj;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        if (available.Count == 0)
        {
            if (!expandable)
                return null;

            CreateObject();
        }

        GameObject obj = available.Dequeue();
        inUse.Add(obj);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
            poolable.OnSpawn();

        return obj;
    }

    public void Return(PoolMember member)
    {
        GameObject obj = member.gameObject;

        if (!inUse.Contains(obj))
            return;

        IPoolable poolable = obj.GetComponent<IPoolable>();
        if (poolable != null)
            poolable.OnDespawn();

        obj.SetActive(false);

        inUse.Remove(obj);
        available.Enqueue(obj);
    }

    public int ActiveCount { get { return inUse.Count; } }
    public int AvailableCount { get { return available.Count; } }
}
