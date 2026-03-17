using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HadesController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 20f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Gravity")]
    public float gravity = -20f;

    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 dashDirection;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleDash();
        HandleMovement();
        ApplyGravity();
    }

    void HandleMovement()
    {
        if (isDashing) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude >= 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * input.z + camRight * input.x;

            // Rotation rapide (style Hades)
            transform.forward = moveDir;

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);

            // Stock direction pour dash
            dashDirection = moveDir.normalized;
        }
    }

    void HandleDash()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            if (dashDirection == Vector3.zero)
                dashDirection = transform.forward;
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
                isDashing = false;
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}