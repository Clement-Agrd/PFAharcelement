using UnityEngine;

public abstract class PoolableObject : MonoBehaviour, IPoolable
{
    protected PoolMember poolMember;

    protected virtual void Awake()
    {
        poolMember = GetComponent<PoolMember>();
    }

    public virtual void OnSpawn() { }
    public virtual void OnDespawn() { }

    public void ReturnToPool()
    {
        poolMember?.ReturnToPool();
    }
}