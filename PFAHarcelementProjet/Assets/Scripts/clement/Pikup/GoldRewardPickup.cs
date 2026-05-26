using UnityEngine;

public class GoldRewardPickup : MonoBehaviour
{
    [Header("Gold")]
    public int goldAmount = 10;

    private bool picked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        picked = true;

        GiveGold();

        Destroy(gameObject);
    }

    void GiveGold()
    {
        // ✅ Ajoute l’or
        if (XPManager.Instance != null)
        {
            XPManager.Instance.AddGold(goldAmount);
        }

        // ✅ Notifie le StageManager (IMPORTANT)
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnRewardPicked();
        }
    }
}
