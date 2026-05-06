using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeButton : MonoBehaviour, IPointerDownHandler
{
    public PlayerCombat playerCombat;

    public void OnPointerDown(PointerEventData eventData)
    {
        playerCombat.TriggerMelee();
    }
}