using UnityEngine;

public class ShopRoomEndTrigger : MonoBehaviour, IRoomEnter
{
    private bool triggered = false;

    public void OnRoomEnter()
    {
        triggered = true;
        StageManager.Instance.OnRoomEnd();
    }
}