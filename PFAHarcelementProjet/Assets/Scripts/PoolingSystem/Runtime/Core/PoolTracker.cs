using System.Collections.Generic;

public static class PoolTracker
{
    private static Dictionary<string, ObjectPool> pools =
        new Dictionary<string, ObjectPool>();

    public static void Register(string id, ObjectPool pool)
    {
        if (!pools.ContainsKey(id))
            pools.Add(id, pool);
    }

    public static ObjectPool Get(string id)
    {
        ObjectPool pool;
        pools.TryGetValue(id, out pool);
        return pool;
    }
}