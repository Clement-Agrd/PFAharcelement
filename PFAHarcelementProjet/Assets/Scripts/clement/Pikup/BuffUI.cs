using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffUI : MonoBehaviour
{
    public static BuffUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI buffNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText; // ✅ AJOUT

    [Header("Shop Colors")]
    public Color affordableColor = Color.white;
    public Color notEnoughColor  = Color.goldenRod;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void SetPickup(RewardPickup pickup)
    {
        if (pickup == null)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);

        // ✅ Infos de base
        buffNameText.text = pickup.buffData.buffName;
        descriptionText.text = pickup.buffData.description;

        // ✅ CAS SHOP
        if (pickup.pickupMode == PickupMode.Shop)
        {
            int playerGold = XPManager.Instance != null 
                ? XPManager.Instance.GetGold() 
                : 0;

            bool canBuy = playerGold >= pickup.price;

            priceText.gameObject.SetActive(true);
            priceText.text = $"Prix : {pickup.price}";

            priceText.color = canBuy 
                ? affordableColor 
                : notEnoughColor;
        }
        else
        {
            // ✅ PAS SHOP → on cache le prix
            priceText.gameObject.SetActive(false);
        }
    }
}