using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "RogueLike/Choice Visual Database")]
public class RoomChoiceVisualDatabase : ScriptableObject
{
    [System.Serializable]
    public class RoomVisualEntry
    {
        public RoomType roomType;
        public GameObject prefab;
    }

    [System.Serializable]
    public class RewardVisualEntry
    {
        public RewardType rewardType;
        public GameObject prefab;
    }

    public RoomVisualEntry[] roomVisuals;
    public RewardVisualEntry[] rewardVisuals;

    private Dictionary<RoomType, GameObject> roomDict;
    private Dictionary<RewardType, GameObject> rewardDict;

    public void Init()
    {
        roomDict = new Dictionary<RoomType, GameObject>();
        rewardDict = new Dictionary<RewardType, GameObject>();

        foreach (var e in roomVisuals)
            roomDict[e.roomType] = e.prefab;

        foreach (var e in rewardVisuals)
            rewardDict[e.rewardType] = e.prefab;
    }

    public GameObject GetRoomVisual(RoomType type)
        => roomDict.ContainsKey(type) ? roomDict[type] : null;

    public GameObject GetRewardVisual(RewardType type)
        => rewardDict.ContainsKey(type) ? rewardDict[type] : null;
}