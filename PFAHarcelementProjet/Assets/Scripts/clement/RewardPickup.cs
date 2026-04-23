using UnityEngine;

public class RewardPickup : MonoBehaviour
{
    public RewardType rewardType;

    [Header("Item Data")]
    public BuffPickupData buffData;

    private bool picked = false;
    
    [Header("Gold")]
    public int goldAmount = 10;


    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        picked = true;

        ApplyReward(other.gameObject);

        StageManager.Instance.SpawnRoomChoices();
        Destroy(gameObject);
    }

   

    private void ApplyReward(GameObject player)
    {
        switch (rewardType)
        {
            case RewardType.Item:
                player.GetComponent<PlayerStats>().ApplyBuff(buffData);
                break;

            case RewardType.Gold:
                XPManager.Instance.AddGold(goldAmount);
                break;
        }
    }
}