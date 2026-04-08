using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour,
    IDragHandler,
    IPointerUpHandler,
    IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;

    private Vector2 input = Vector2.zero;

    public Vector2 Input => input;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos.x = pos.x / (background.sizeDelta.x);
        pos.y = pos.y / (background.sizeDelta.y);

        input = new Vector2(pos.x * 2, pos.y * 2);

        input = input.magnitude > 1 ? input.normalized : input;

        handle.anchoredPosition = new Vector2(
            input.x * background.sizeDelta.x / 2,
            input.y * background.sizeDelta.y / 2
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}