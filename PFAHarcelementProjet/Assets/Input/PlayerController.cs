using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Vector2 moveInput;
    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;

    void Awake()
    {
        controls = new PlayerControls();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Dash.performed += ctx => TryDash();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleDash();
    }

    void HandleMovement()
    {
        if (isDashing) return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Isométrique → rotation 45°
        move = Quaternion.Euler(0, 45, 0) * move;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void TryDash()
    {
        if (cooldownTimer > 0 || isDashing) return;

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
    }

    void HandleDash()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = Quaternion.Euler(0, 45, 0) * move;

        controller.Move(move.normalized * dashSpeed * Time.deltaTime);

        if (dashTimer <= 0)
            isDashing = false;
    }
}