using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public VirtualJoystick aimJoystick; // mobile
    public Vector2 MoveInput => moveInput;
    

    PlayerControls controls;

    Vector2 aimInput;
    Vector2 moveInput;
    bool shootPressed;
    bool meleePressed;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += _  => moveInput = Vector2.zero;

        // AIM (manette / souris delta / joystick virtuel)
        controls.Player.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled += _ => aimInput = Vector2.zero;

        // SHOOT
        controls.Player.Shoot.performed += _ => shootPressed = true;
        controls.Player.Shoot.canceled += _ => shootPressed = false;

        // MELEE
        controls.Player.Attack.performed += _ => meleePressed = true;
        controls.Player.Attack.canceled += _ => meleePressed = false;
    }

    public Vector2 AimInput =>
        aimJoystick != null && aimJoystick.Input.magnitude > 0.2f
            ? aimJoystick.Input
            : aimInput;

    public bool ShootPressed => shootPressed;
    public bool MeleePressed => meleePressed;
}