using UnityEngine;

[CreateAssetMenu(fileName = "StageLayout", menuName = "RogueLike/Stage Layout")]
public class StageLayout : ScriptableObject
{
    public RoomType[] rooms;
}