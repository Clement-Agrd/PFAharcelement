using UnityEngine;

public class ShopRoomController : MonoBehaviour, IRoomEnter, IRoomExit
{
    public ItemPrefabPool itemPrefabPool;
    public Transform[] itemSpawnPoints;

    public void OnRoomEnter()
    {
        Debug.Log("✅ ShopRoomController.OnRoomEnter appelée");
        SpawnShopItems();
    }

    void SpawnShopItems()
    {
        for (int i = 0; i < 3 && i < itemSpawnPoints.Length; i++)
        {
            GameObject prefab = itemPrefabPool.GetRandomItemPrefab();
            if (prefab == null) continue;

            GameObject obj = Instantiate(
                prefab,
                itemSpawnPoints[i].position,
                Quaternion.identity,
                transform // ✅ PARENT = SALLE
            );

            RewardPickup pickup = obj.GetComponent<RewardPickup>();
            pickup.pickupMode = PickupMode.Shop;
        }
    }

    public void OnRoomExit()
    {
        Debug.Log("🧹 ShopRoomController.OnRoomExit appelée");

        foreach (Transform child in transform)
        {
            RewardPickup pickup = child.GetComponent<RewardPickup>();
            if (pickup != null && pickup.pickupMode == PickupMode.Shop)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
