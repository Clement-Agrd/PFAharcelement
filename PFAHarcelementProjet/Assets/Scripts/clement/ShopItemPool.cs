using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Roguelike/Shop Item Pool")]
public class ShopItemPool : ScriptableObject
{
    public List<GameObject> itemPrefabs;

    public GameObject GetRandomItem()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0)
            return null;

        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }
}