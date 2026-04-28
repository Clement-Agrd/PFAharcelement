using UnityEngine;

[CreateAssetMenu(fileName = "StageLayout", menuName = "RogueLike/Stage Layout")]
public class StageLayout : ScriptableObject
{
    public StageNode[] nodes;
}

[System.Serializable]
public class StageNode
{
    public RoomChoice[] choices;
}
