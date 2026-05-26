using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public static StageManager Instance;

    private List<GameObject> spawnedChoices = new List<GameObject>();


    public StageLayout layout;
    private int index = 0;
    private RoomChoice pendingChoice;
    
    private RoomSpawnPoints currentRoomSpawnPoints;
    
    [Header("World Choices")]
    public GameObject choicePrefab;
    
    [Header("Reward Prefabs")]
    public GameObject goldRewardPrefab;

    

    [Header("Item Rewards")]
   
    [Header("Item Pools")]
    public ItemPrefabPool combatItemPool;
    public ItemPrefabPool eliteItemPool;
    public ItemPrefabPool bossItemPool;
    
    
    [Header("Exit")]
    public GameObject nextStagePortalPrefab;



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

        spawnedChoices.Clear();

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

            spawnedChoices.Add(obj);
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
        if (pendingChoice == null || currentRoomSpawnPoints == null)
            return;

        switch (pendingChoice.rewardType)
        {
            case RewardType.Gold:
                Instantiate(
                    goldRewardPrefab,
                    currentRoomSpawnPoints.rewardSpawnPoint.position,
                    Quaternion.identity
                );
                break;

            case RewardType.Item:
                SpawnRandomItemPrefab();
                break;

            case RewardType.None:
                SpawnRoomChoices();
                break;
        }
    }
    
    
    void SpawnRandomItemPrefab()
    {
        ItemPrefabPool pool = GetPoolForRoomType(pendingChoice.roomType);
        if (pool == null)
        {
            SpawnRoomChoices();
            return;
        }

        GameObject itemPrefab = pool.GetRandomItemPrefab();
        if (itemPrefab == null)
        {
            SpawnRoomChoices();
            return;
        }

        GameObject obj = Instantiate(
            itemPrefab,
            currentRoomSpawnPoints.rewardSpawnPoint.position,
            Quaternion.identity
        );

        RewardPickup pickup = obj.GetComponent<RewardPickup>();
        pickup.pickupMode = PickupMode.Reward;
    }
    
    
    public void OnRewardPicked()
    {
        // ✅ Si dernière room → portail
        if (index >= layout.nodes.Length)
        {
            SpawnExitPortal();
        }
        else
        {
            SpawnRoomChoices();
        }
    }

    void SpawnExitPortal()
    {
        if (currentRoomSpawnPoints == null || nextStagePortalPrefab == null)
            return;

        Transform spawnPoint = currentRoomSpawnPoints.exitSpawnPoint;

        // ✅ fallback si non assigné
        if (spawnPoint == null)
            spawnPoint = currentRoomSpawnPoints.rewardSpawnPoint;

        Instantiate(
            nextStagePortalPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }

    ItemPrefabPool GetPoolForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Elite:
                return eliteItemPool;

            case RoomType.Boss:
                return bossItemPool;

            case RoomType.Combat:
            default:
                return combatItemPool;
        }
    }

    
    public void ClearRoomChoices()
    {
        foreach (var choice in spawnedChoices)
        {
            if (choice != null)
                Destroy(choice);
        }

        spawnedChoices.Clear();
    }

}