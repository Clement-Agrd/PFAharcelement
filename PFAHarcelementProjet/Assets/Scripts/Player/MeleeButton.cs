using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeButton : MonoBehaviour, IPointerDownHandler
{
    public PlayerMelee playerMelee;

    public void OnPointerDown(PointerEventData eventData)
    {
        playerMelee.TryAttack();
    }
}