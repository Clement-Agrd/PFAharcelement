using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;

    private Vector2 input = Vector2.zero;
    public Vector2 Input => input;

    void Start()
    {
        if (background == null)
            background = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out position))
        {
            position.x /= background.sizeDelta.x;
            position.y /= background.sizeDelta.y;

            input = new Vector2(position.x * 2, position.y * 2);
            input = (input.magnitude > 1) ? input.normalized : input;

            handle.anchoredPosition = new Vector2(
                input.x * (background.sizeDelta.x / 3),
                input.y * (background.sizeDelta.y / 3)
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}