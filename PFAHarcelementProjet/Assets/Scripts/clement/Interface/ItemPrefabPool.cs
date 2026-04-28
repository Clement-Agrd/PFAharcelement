using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Roguelike/Item Prefab Pool")]
public class ItemPrefabPool : ScriptableObject
{
    public List<GameObject> itemPrefabs;

    public GameObject GetRandomItemPrefab()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0)
            return null;

        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }
}
