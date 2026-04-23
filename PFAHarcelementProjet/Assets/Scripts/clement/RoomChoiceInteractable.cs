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
        {
            GameObject roomInstance =
                Instantiate(roomVisual, roomVisualRoot);

            roomInstance.transform.localPosition = Vector3.zero;
            roomInstance.transform.localRotation = Quaternion.identity;
        }

        GameObject rewardVisual = db.GetRewardVisual(choice.rewardType);
        if (rewardVisual != null)
        {
            GameObject rewardInstance =
                Instantiate(rewardVisual, rewardVisualRoot);

            rewardInstance.transform.localPosition = Vector3.zero;
            rewardInstance.transform.localRotation = Quaternion.identity;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        StageManager.Instance.ClearRoomChoices();
        StageManager.Instance.SelectChoice(choice);
    }
}
