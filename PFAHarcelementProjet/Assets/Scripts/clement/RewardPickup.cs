using UnityEngine;

public class RewardPickup : MonoBehaviour
{
    public RewardType rewardType;

    private bool picked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        picked = true;

        ApplyReward();
        StageManager.Instance.SpawnRoomChoices();

        Destroy(gameObject);
    }

    private void ApplyReward()
    {
        switch (rewardType)
        {
            case RewardType.Gold:
                Debug.Log("Ajout d’or au joueur");
                // PlayerStats.Instance.AddGold(x);
                break;

            case RewardType.Item:
                Debug.Log("Ajout d’un item");
                // Inventory.Instance.AddItem(...)
                break;
        }
    }
}