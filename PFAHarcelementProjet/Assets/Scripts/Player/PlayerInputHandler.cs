using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public VirtualJoystick moveJoystick; // mobile déplacement
    public VirtualJoystick aimJoystick;  // mobile visée

    PlayerControls controls;

    Vector2 moveInput;
    Vector2 aimInput;
    bool shootPressed;
    bool meleePressed;
    bool dashPressed;
    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();

        // MOVE
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += _  => moveInput = Vector2.zero;

        // AIM (manette)
        controls.Player.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled  += _  => aimInput = Vector2.zero;

        // SHOOT
        controls.Player.Shoot.performed += _ => shootPressed = true;
        controls.Player.Shoot.canceled  += _ => shootPressed = false;

        // MELEE
        controls.Player.Attack.performed += _ => meleePressed = true;
        controls.Player.Attack.canceled  += _ => meleePressed = false;

        controls.Player.Dash.performed += _ => dashPressed = true;
        controls.Player.Dash.canceled  += _ => dashPressed = false;
    }

    // ✅ MOVE
    public Vector2 MoveInput =>
        moveJoystick != null && moveJoystick.Input.magnitude > 0.2f
            ? moveJoystick.Input
            : moveInput;

    // ✅ AIM
    public Vector2 AimInput =>
        aimJoystick != null && aimJoystick.Input.magnitude > 0.2f
            ? aimJoystick.Input
            : aimInput;

    public bool ShootPressed => shootPressed;
    public bool MeleePressed => meleePressed;
    public bool DashPressed => dashPressed;
}