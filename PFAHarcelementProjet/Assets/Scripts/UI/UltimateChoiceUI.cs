// Scripts/UI/UltimateChoiceUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateChoiceUI : MonoBehaviour
{
    [Header("Références")]
    public GameObject           panelChoice;
    public List<UltimateCardUI> cards = new List<UltimateCardUI>();

    void Awake()
    {
        Debug.Log("✅ UltimateChoiceUI Awake");

        if (panelChoice == null)
            Debug.LogError("❌ UltimateChoiceUI : panelChoice NON assigné");

        if (cards == null || cards.Count == 0)
            Debug.LogError("❌ UltimateChoiceUI : aucune carte assignée");

        if (panelChoice != null)
            panelChoice.SetActive(false);
    }

    public void Show(List<UltimateData> choices)
    {
        Debug.Log($"🎴 UltimateChoiceUI.Show — {choices?.Count} choix");

        if (panelChoice == null)
        {
            Debug.LogError("❌ panelChoice null");
            return;
        }

        if (choices == null || choices.Count == 0)
        {
            Debug.LogError("❌ Aucun ultime dans UltimateManager.allUltimates");
            return;
        }

        panelChoice.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null)
            {
                Debug.LogError($"❌ Carte {i} non assignée");
                continue;
            }

            if (i < choices.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(choices[i], this);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnChoose(UltimateData data)
    {
        Debug.Log($"✅ Ultime choisi : {data.ultimateName}");

        UltimateManager.Instance.ApplyUltimate(data);

        if (panelChoice != null)
            panelChoice.SetActive(false);

        Time.timeScale = 1f;
    }
}