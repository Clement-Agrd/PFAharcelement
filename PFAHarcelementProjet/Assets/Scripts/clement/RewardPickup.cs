using UnityEngine;

public class RewardPickup : MonoBehaviour
{
    [Header("Common")]
    public BuffPickupData buffData;
    public PickupMode pickupMode = PickupMode.Reward;

    [Header("Shop Only")]
    public int price = 0;

    private bool picked = false;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        if (pickupMode == PickupMode.Shop)
        {
            TryBuy(other.gameObject);
        }
        else
        {
            GiveReward(other.gameObject);
        }
    }

    // 🎁 Reward gratuit
    void GiveReward(GameObject player)
    {
        picked = true;

        player.GetComponent<PlayerStats>().ApplyBuff(buffData);
        StageManager.Instance.SpawnRoomChoices();

        Destroy(gameObject);
    }

    // 🛒 Shop
    void TryBuy(GameObject player)
    {
        if (XPManager.Instance.GetGold() < price)
        {
            Debug.Log($"❌ Pas assez d'or (coût : {price})");
            return;
        }

        picked = true;

        XPManager.Instance.AddGold(-price);
        player.GetComponent<PlayerStats>().ApplyBuff(buffData);

        Debug.Log($"✅ Item acheté : {buffData.buffName} (-{price} or)");
        Destroy(gameObject);
    }
}