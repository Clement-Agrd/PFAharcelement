using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class VirtualCursor : MonoBehaviour
{
    [Header("Références")]
    public RectTransform cursorTransform;
    public Image         cursorImage;
    public Canvas        canvas;

    [Header("Paramètres")]
    public float cursorSpeed = 800f;
    public float padding     = 10f;

    private Mouse         virtualMouse;
    private Camera        mainCamera;
    private RectTransform canvasRect;
    private bool          isGamepad  = false;
    private EventSystem   eventSystem;
    private GameObject    lastHovered;
    private bool          isDragging  = false;

    void Awake()
    {
        mainCamera  = Camera.main;
        canvasRect  = canvas.GetComponent<RectTransform>();
        eventSystem = EventSystem.current;

        // Cache tout dès le départ
        if (cursorTransform != null)
            cursorTransform.gameObject.SetActive(false);

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnEnable()
    {
        // Recrée la souris virtuelle proprement
        if (virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");

        InputSystem.onAfterUpdate  += UpdateVirtualMouse;
        InputSystem.onDeviceChange += OnDeviceChange;

        // Recentre
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        InputState.Change(virtualMouse.position, center);

        isDragging  = false;
        lastHovered = null;

        // Affiche le curseur visuel
        if (cursorTransform != null)
            cursorTransform.gameObject.SetActive(true);

        // Détecte le mode actuel
        SetGamepadMode(Gamepad.current != null);

        AnchorCursor(center);
    }

    void OnDisable()
    {
        InputSystem.onAfterUpdate  -= UpdateVirtualMouse;
        InputSystem.onDeviceChange -= OnDeviceChange;

        if (virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        virtualMouse = null;

        // Cache le curseur visuel
        if (cursorTransform != null)
            cursorTransform.gameObject.SetActive(false);

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;

        isDragging  = false;
        lastHovered = null;

        if (eventSystem != null)
            eventSystem.SetSelectedGameObject(null);
    }

    // ─── Détection périphérique ───────────────────────────────────────────────

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        bool added   = change == InputDeviceChange.Added ||
                       change == InputDeviceChange.Reconnected;
        bool removed = change == InputDeviceChange.Removed ||
                       change == InputDeviceChange.Disconnected;

        if (device is Gamepad)
        {
            if (added)   SetGamepadMode(true);
            if (removed) SetGamepadMode(Gamepad.current != null);
        }

        if (device is Mouse && added)
            SetGamepadMode(false);
    }

    void SetGamepadMode(bool gamepad)
    {
        isGamepad = gamepad;

        // Affiche/cache via alpha — le gameObject reste actif
        if (cursorImage != null)
        {
            Color c = cursorImage.color;
            c.a = gamepad ? 1f : 0f;
            cursorImage.color = c;
        }

        Cursor.visible   = !gamepad;
        Cursor.lockState = CursorLockMode.None;

        if (eventSystem != null)
            eventSystem.SetSelectedGameObject(null);
    }

    // ─── Mise à jour souris virtuelle ─────────────────────────────────────────

    void UpdateVirtualMouse()
    {
        if (virtualMouse == null) return;

        // Bascule vers souris si la souris bouge
        if (isGamepad && Mouse.current != null &&
            Mouse.current.delta.ReadValue().magnitude > 0.5f)
        {
            SetGamepadMode(false);
            return;
        }

        // Bascule vers manette si le stick bouge
        if (!isGamepad && Gamepad.current != null &&
            Gamepad.current.leftStick.ReadValue().magnitude > 0.2f)
            SetGamepadMode(true);

        if (!isGamepad) return;
        if (Gamepad.current == null) return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        if (stick.magnitude < 0.15f) stick = Vector2.zero;

        Vector2 currentPos = virtualMouse.position.ReadValue();
        Vector2 newPos     = currentPos + stick * cursorSpeed * Time.unscaledDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width  - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta,
            stick * cursorSpeed * Time.unscaledDeltaTime);

        bool crossPressed = Gamepad.current.buttonSouth.isPressed;
        bool wasPressed   = virtualMouse.leftButton.isPressed;

        if (crossPressed != wasPressed)
        {
            using (StateEvent.From(virtualMouse, out InputEventPtr eventPtr))
            {
                virtualMouse.leftButton.WriteValueIntoEvent(
                    crossPressed ? 1f : 0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }

            if (crossPressed) OnPress(newPos);
            else              OnRelease(newPos);
        }

        if (crossPressed && isDragging)
            OnDrag(newPos);

        ProcessRaycast(newPos);
        AnchorCursor(newPos);
    }

    // ─── Presse / Relâche / Drag ──────────────────────────────────────────────

    void OnPress(Vector2 screenPos)
    {
        if (eventSystem == null) return;

        List<RaycastResult> results = RaycastAllCanvases(screenPos);
        if (results.Count == 0) return;

        GameObject interactable = FindInteractable(results[0].gameObject);
        if (interactable == null) return;

        lastHovered = interactable;
        isDragging  = true;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        { position = screenPos };

        ExecuteEvents.Execute(interactable, pointerData,
            ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(interactable, pointerData,
            ExecuteEvents.beginDragHandler);

        eventSystem.SetSelectedGameObject(interactable);
    }

    void OnRelease(Vector2 screenPos)
    {
        if (lastHovered == null) return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        { position = screenPos };

        ExecuteEvents.Execute(lastHovered, pointerData,
            ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(lastHovered, pointerData,
            ExecuteEvents.endDragHandler);
        ExecuteEvents.Execute(lastHovered, pointerData,
            ExecuteEvents.pointerClickHandler);

        isDragging  = false;
        lastHovered = null;
    }

    void OnDrag(Vector2 screenPos)
    {
        if (lastHovered == null) return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos,
            delta    = Gamepad.current.leftStick.ReadValue() *
                       cursorSpeed * Time.unscaledDeltaTime
        };

        ExecuteEvents.Execute(lastHovered, pointerData,
            ExecuteEvents.dragHandler);
    }

    // ─── Raycast sur tous les Canvas ─────────────────────────────────────────

    List<RaycastResult> RaycastAllCanvases(Vector2 screenPos)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        { position = screenPos };

        List<RaycastResult> allResults = new List<RaycastResult>();

        foreach (GraphicRaycaster raycaster in FindObjectsOfType<GraphicRaycaster>())
        {
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);
            allResults.AddRange(results);
        }

        allResults.Sort((a, b) =>
        {
            int canvasCompare = b.sortingOrder.CompareTo(a.sortingOrder);
            if (canvasCompare != 0) return canvasCompare;
            return b.depth.CompareTo(a.depth);
        });

        return allResults;
    }

    void ProcessRaycast(Vector2 screenPos)
    {
        if (eventSystem == null) return;

        List<RaycastResult> results = RaycastAllCanvases(screenPos);

        if (results.Count > 0)
        {
            GameObject interactable = FindInteractable(results[0].gameObject);
            if (interactable != null && !isDragging &&
                interactable != eventSystem.currentSelectedGameObject)
                eventSystem.SetSelectedGameObject(interactable);
        }
        else if (!isDragging)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    GameObject FindInteractable(GameObject go)
    {
        Transform current = go.transform;
        while (current != null)
        {
            if (current.GetComponent<Button>()       != null) return current.gameObject;
            if (current.GetComponent<Slider>()       != null) return current.gameObject;
            if (current.GetComponent<Toggle>()       != null) return current.gameObject;
            if (current.GetComponent<Scrollbar>()    != null) return current.gameObject;
            if (current.GetComponent<TMP_Dropdown>() != null) return current.gameObject;
            if (current.GetComponent<ScrollRect>()   != null) return current.gameObject;
            current = current.parent;
        }
        return null;
    }

    void AnchorCursor(Vector2 screenPos)
    {
        if (cursorTransform == null || canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : mainCamera,
            out Vector2 localPoint
        );

        cursorTransform.localPosition = localPoint;
    }
}