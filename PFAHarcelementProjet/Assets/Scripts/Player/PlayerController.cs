
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float gravity       = -20f;

    [Header("Dash mêlée")]
    public float dashSpeed    = 18f;
    public float dashDuration = 0.12f;

    private CharacterController controller;
    private PlayerInputHandler  input;
    private PlayerStats         stats;
    private float               verticalVelocity;

    private bool    isDashing;
    private Vector3 dashDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input      = GetComponent<PlayerInputHandler>();
        stats      = GetComponent<PlayerStats>();
    }

    void Update()
    {
        ApplyGravity();

        if (!isDashing)
            Move();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime * 0.1f;
    }

    void Move()
    {
        float moveSpeed = stats.GetStat(StatType.MoveSpeed);

        Vector2 moveInput = input.MoveInput;
        Vector3 move      = new Vector3(moveInput.x, 0, moveInput.y);

        move.y = verticalVelocity;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    // ─── Dash appelé par PlayerMelee ─────────────────────────────────────────

    public void StartDash(Vector3 direction)
    {
        if (isDashing) return;
        StartCoroutine(DashCoroutine(direction));
    }

    IEnumerator DashCoroutine(Vector3 direction)
    {
        isDashing = true;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            Vector3 move = direction * dashSpeed;
            move.y = verticalVelocity;
            controller.Move(move * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    public bool IsDashing => isDashing;

    public void SetMovement(bool enabled)
    {
        verticalVelocity = -2f;
        this.enabled = enabled;
    }
}