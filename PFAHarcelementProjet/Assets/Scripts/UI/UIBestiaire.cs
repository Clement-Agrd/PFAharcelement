using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestiaryUI : MonoBehaviour
{
    [Header("Toutes les créatures")]
    public BestiaryEntry[] allCreatures;

    [Header("UI")]
    public Image creatureImage;
    public TMP_Text creatureName;
    public TMP_Text description;

    [Header("Image inconnue")]
    public Sprite unknownSprite;

    private int currentPage = 0;

    void OnEnable()
    {
        RefreshCurrentPage();
    }

    public void ShowPage(int index)
    {
        if (allCreatures == null || allCreatures.Length == 0)
            return;

        if (index < 0)
            index = allCreatures.Length - 1;

        if (index >= allCreatures.Length)
            index = 0;

        currentPage = index;

        BestiaryEntry entry = allCreatures[currentPage];

        if (BestiaryManager.Instance != null &&
            BestiaryManager.Instance.IsUnlocked(entry.id))
        {
            creatureImage.sprite = entry.image;
            creatureName.text = entry.creatureName;
            description.text = entry.description;
        }
        else
        {
            creatureImage.sprite = unknownSprite;
            creatureName.text = "???";
            description.text = "Créature inconnue";
        }
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }

    public void RefreshCurrentPage()
    {
        ShowPage(currentPage);
    }
}