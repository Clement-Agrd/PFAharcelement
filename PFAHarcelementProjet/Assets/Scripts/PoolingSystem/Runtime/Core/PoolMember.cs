using UnityEngine;

[DisallowMultipleComponent]
public class PoolMember : MonoBehaviour
{
    public string PoolID { get; private set; }

    public void SetPool(string id)
    {
        PoolID = id;
    }

    public void ReturnToPool()
    {
        if (PoolManager.Instance != null)
            PoolManager.Instance.ReturnToPool(this);
    }
}