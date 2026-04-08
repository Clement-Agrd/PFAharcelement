using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Joysticks")]
    public VirtualJoystick moveJoystick;
    public VirtualJoystick aimJoystick;

    private PlayerControls controls;

    private Vector2 keyboardMove;
    private Vector2 gamepadAim;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += ctx => keyboardMove = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => keyboardMove = Vector2.zero;

        controls.Player.Aim.performed += ctx => gamepadAim = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled += ctx => gamepadAim = Vector2.zero;
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public Vector2 MoveInput
    {
        get
        {
            Vector2 joystick = moveJoystick != null ? moveJoystick.Input : Vector2.zero;

            // Priorité joystick mobile
            if (joystick.magnitude > 0.1f)
                return joystick;

            // Sinon clavier / manette
            return keyboardMove;
        }
    }

    public Vector2 AimInput
    {
        get
        {
            Vector2 joystick = aimJoystick != null ? aimJoystick.Input : Vector2.zero;

            if (joystick.magnitude > 0.1f)
                return joystick;

            return gamepadAim;
        }
    }
}