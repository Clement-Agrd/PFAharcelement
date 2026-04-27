using UnityEngine;


public class ShopRoomController : MonoBehaviour
{
    public ItemPrefabPool itemPrefabPool;
    public Transform[] itemSpawnPoints;

    void Start()
    {
        SpawnShopItems();
    }

    void SpawnShopItems()
    {
        for (int i = 0; i < 3 && i < itemSpawnPoints.Length; i++)
        {
            GameObject prefab = itemPrefabPool.GetRandomItemPrefab();
            GameObject obj = Instantiate(prefab, itemSpawnPoints[i].position, Quaternion.identity);

            RewardPickup pickup = obj.GetComponent<RewardPickup>();
            pickup.pickupMode = PickupMode.Shop;
        }
    }
}
