using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HadesControllerAdvanced : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Dash")]
    public float dashSpeed = 22f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;

    private Vector3 mouseDirection;

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
        AimWithMouse();
        HandleDash();
        HandleMovement();
        ApplyGravity();
    }

    void AimWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            mouseDirection = (point - transform.position).normalized;
            mouseDirection.y = 0;

            if (mouseDirection != Vector3.zero)
                transform.forward = mouseDirection;
        }
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

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
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
        }

        if (isDashing)
        {
            controller.Move(mouseDirection * dashSpeed * Time.deltaTime);

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