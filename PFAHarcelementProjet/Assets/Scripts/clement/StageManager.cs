using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public StageLayout layout;
    private int index = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadNext();
    }

    public void LoadNext()
    {
        if (index >= layout.rooms.Length)
            return;

        RoomLoader.Instance.LoadRoom(layout.rooms[index]);

        index++;
    }
}