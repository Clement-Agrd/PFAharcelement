using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public StageLayout layout;
    private int index = 0;

    private RoomChoice pendingChoice;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowChoices();
    }

    public void ShowChoices()
    {
        if (index >= layout.nodes.Length)
            return;

        StageNode node = layout.nodes[index];

        // 👉 ici tu affiches ton UI de choix
        ChoiceUI.Instance.Show(node.choices);
    }

    public void SelectChoice(RoomChoice choice)
    {
        pendingChoice = choice;

        RoomLoader.Instance.LoadRoom(choice.roomType);
        index++;
    }

    public void GiveReward()
    {
        if (pendingChoice == null) return;

        switch (pendingChoice.rewardType)
        {
            case RewardType.Gold:
                Debug.Log("Donner gold");
                break;
            case RewardType.Item:
                Debug.Log("Donner item");
                break;
        }
    }
}