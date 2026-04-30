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
    public Image icon;

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
        buffNameText.text = pickup.buffData.buffName;
        descriptionText.text = pickup.buffData.description;
        icon.sprite = pickup.buffData.icon;
    }
}