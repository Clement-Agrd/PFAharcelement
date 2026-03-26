using UnityEngine;

public class ObjectPoolGeneric<T> where T : Component
{
    private readonly ObjectPool basePool;

    public ObjectPoolGeneric(ObjectPool pool)
    {
        basePool = pool;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = basePool.Get(position, rotation);
        if (obj == null) return null;

        return obj.GetComponent<T>();
    }
}