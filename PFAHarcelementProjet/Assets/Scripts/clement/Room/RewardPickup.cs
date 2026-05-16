// Scripts/Pickups/RewardPickup.cs
using UnityEngine;

public class RewardPickup : MonoBehaviour
{
    [Header("Common")]
    public BuffPickupData buffData;
    public PickupMode     pickupMode = PickupMode.Reward;

    [Header("Shop Only")]
    public int price = 0;

    private bool picked = false;

    void OnTriggerEnter(Collider other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        switch (pickupMode)
        {
            case PickupMode.Reward:
                GiveReward(other.gameObject);
                break;

            case PickupMode.Shop:
                TryBuy(other.gameObject);
                break;

            case PickupMode.SpecialAttackUnlock:
                UnlockSpecialAttack(other.gameObject);
                break;
        }
    }

    // ─── Reward gratuit ───────────────────────────────────────────────────────

    void GiveReward(GameObject player)
    {
        picked = true;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null && buffData != null)
            stats.ApplyBuff(buffData);

        StageManager.Instance.SpawnRoomChoices();
        Destroy(gameObject);
    }

    // ─── Shop ─────────────────────────────────────────────────────────────────

    void TryBuy(GameObject player)
    {
        if (XPManager.Instance.GetGold() < price)
        {
            Debug.Log($"❌ Pas assez d'or (coût : {price})");
            return;
        }

        picked = true;

        XPManager.Instance.AddGold(-price);

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null && buffData != null)
            stats.ApplyBuff(buffData);

        Debug.Log($"✅ Item acheté : {buffData.buffName} (-{price} or)");
        Destroy(gameObject);
    }

    // ─── Déblocage attaque spéciale ───────────────────────────────────────────

    void UnlockSpecialAttack(GameObject player)
    {
        SpecialAttack special = player.GetComponent<SpecialAttack>();

        if (special == null)
        {
            Debug.LogWarning("⚠️ SpecialAttack introuvable sur le joueur");
            return;
        }

        if (special.IsUnlocked)
        {
            Debug.Log("✅ Attaque spéciale déjà débloquée");
            return;
        }

        picked = true;
        special.Unlock();

        Debug.Log("🔓 Attaque spéciale débloquée !");
        Destroy(gameObject);
    }
}