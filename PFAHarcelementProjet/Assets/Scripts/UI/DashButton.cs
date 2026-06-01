using UnityEngine;
using UnityEngine.EventSystems;

public class DashButton : MonoBehaviour, IPointerDownHandler
{
    public PlayerCombat playerCombat;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerCombat == null) return;

        playerCombat.TriggerDash();
    }
}
