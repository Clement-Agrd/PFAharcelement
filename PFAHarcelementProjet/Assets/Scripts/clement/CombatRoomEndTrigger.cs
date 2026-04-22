using UnityEngine;

public class CombatRoomEndTrigger : MonoBehaviour
{
    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            triggered = true;
            StageManager.Instance.OnRoomEnd();
        }
    }
}
