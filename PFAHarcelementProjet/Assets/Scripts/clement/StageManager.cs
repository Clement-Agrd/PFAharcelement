using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public StageLayout layout;
    private int index = 0;
    private RoomChoice pendingChoice;
    
    private RoomSpawnPoints currentRoomSpawnPoints;
    
    [Header("World Choices")]
    public GameObject choicePrefab;
    
    [Header("Reward Prefabs")]
    public GameObject goldRewardPrefab;
    public GameObject itemRewardPrefab;

    public RoomChoiceVisualDatabase choiceVisualDatabase;
    

  

    void Awake()
    {
        Instance = this;

        if (choiceVisualDatabase != null)
            choiceVisualDatabase.Init();
    }



    void Start()
    {
        // Prend automatiquement le premier choix du premier node
        if (layout.nodes.Length > 0 && layout.nodes[0].choices.Length > 0)
        {
            SelectChoice(layout.nodes[0].choices[0]);
        }
    }

    public void ShowChoices()
    {
        if (index >= layout.nodes.Length)
            return;

        StageNode node = layout.nodes[index];
        
    }
    public void OnRoomEnd()
    {
        if (pendingChoice == null) return;
        GiveReward();
    }
    
    public void SpawnRoomChoices()
    {
        if (index >= layout.nodes.Length || currentRoomSpawnPoints == null)
            return;

        StageNode node = layout.nodes[index];

        for (int i = 0;
             i < node.choices.Length &&
             i < currentRoomSpawnPoints.choiceSpawnPoints.Length;
             i++)
        {
            GameObject obj = Instantiate(
                choicePrefab,
                currentRoomSpawnPoints.choiceSpawnPoints[i].position,
                Quaternion.identity
            );

            obj.GetComponent<RoomChoiceInteractable>()
                .Init(node.choices[i], choiceVisualDatabase);
        }
    }

    public void RegisterRoom(GameObject room)
    {
        currentRoomSpawnPoints = room.GetComponent<RoomSpawnPoints>();
    }
    
    public void SelectChoice(RoomChoice choice)
    {
        pendingChoice = choice;

        RoomLoader.Instance.LoadRoom(choice.roomType);
        index++;
    }

   
    public void GiveReward()
    {
        if (pendingChoice == null || currentRoomSpawnPoints == null) return;

        GameObject rewardPrefab = null;

        switch (pendingChoice.rewardType)
        {
            case RewardType.Gold:
                rewardPrefab = goldRewardPrefab;
                break;

            case RewardType.Item:
                rewardPrefab = itemRewardPrefab;
                break;

            case RewardType.None:
                SpawnRoomChoices();
                return;
        }

        Instantiate(
            rewardPrefab,
            currentRoomSpawnPoints.rewardSpawnPoint.position,
            Quaternion.identity
        );
    }
}