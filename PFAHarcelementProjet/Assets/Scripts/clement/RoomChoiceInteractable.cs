using UnityEngine;

public class RoomChoiceInteractable : MonoBehaviour
{
    public Transform roomVisualRoot;
    public Transform rewardVisualRoot;

    private RoomChoice choice;
    private bool used = false;

    public void Init(RoomChoice choice, RoomChoiceVisualDatabase db)
    {
        this.choice = choice;

        GameObject roomVisual = db.GetRoomVisual(choice.roomType);
        if (roomVisual != null)
            Instantiate(roomVisual, roomVisualRoot);

        GameObject rewardVisual = db.GetRewardVisual(choice.rewardType);
        if (rewardVisual != null)
            Instantiate(rewardVisual, rewardVisualRoot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        StageManager.Instance.SelectChoice(choice);
    }
}
