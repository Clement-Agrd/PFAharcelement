using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUI : MonoBehaviour
{
    public static ChoiceUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    public void Show(RoomChoice[] choices)
    {
        panel.SetActive(true);

        // Nettoyer anciens boutons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Créer nouveaux boutons
        foreach (RoomChoice choice in choices)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);

            TMP_Text text = btnObj.GetComponentInChildren<TMP_Text>();
            text.text = choice.roomType + " + " + choice.rewardType;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnClickChoice(choice));
        }
    }

    public void OnClickChoice(RoomChoice choice)
    {
        panel.SetActive(false);
        StageManager.Instance.SelectChoice(choice);
    }
}