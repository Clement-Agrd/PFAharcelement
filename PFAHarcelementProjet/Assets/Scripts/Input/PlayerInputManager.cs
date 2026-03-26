using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Joysticks")]
    public VirtualJoystick moveJoystick;
    public VirtualJoystick aimJoystick;

    public Vector2 Move { get; private set; }
    public Vector2 Aim { get; private set; }

    void Update()
    {
        HandleMove();
        HandleAim();
    }

    void HandleMove()
    {
        Vector2 keyboardMove = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector2 joystickMove = moveJoystick != null ? moveJoystick.Input : Vector2.zero;

        // Synchronisation
        Move = keyboardMove + joystickMove;

        Move = Vector2.ClampMagnitude(Move, 1f);
    }

    void HandleAim()
    {
        Vector2 mouseAim = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        Vector2 joystickAim = aimJoystick != null ? aimJoystick.Input : Vector2.zero;

        Aim = mouseAim + joystickAim;

        Aim = Vector2.ClampMagnitude(Aim, 1f);
    }
}