using UnityEngine;

[CreateAssetMenu(menuName = "ProPooling/Pool Config")]
public class PoolConfig : ScriptableObject
{
    public string poolID;
    public GameObject prefab;
    public int initialSize = 10;
    public bool expandable = true;
}