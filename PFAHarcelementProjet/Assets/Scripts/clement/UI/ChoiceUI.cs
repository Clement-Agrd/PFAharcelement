using UnityEngine;

public class ChoiceUI : MonoBehaviour
{
    public static ChoiceUI Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Show(RoomChoice[] choices)
    {
        foreach (var choice in choices)
        {
            Debug.Log("Choix: " + choice.roomType + " + " + choice.rewardType);
        }

        // 👉 ici tu affiches boutons UI
    }

    public void OnClickChoice(RoomChoice choice)
    {
        StageManager.Instance.SelectChoice(choice);
    }
}