using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float gravity = -20f;

    [Header("Dash mêlée")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.12f;

    private CharacterController controller;
    private PlayerInputHandler input;
    private PlayerStats stats;
    private float verticalVelocity;

    private bool isDashing;

    // ✅ VERROUS GLOBAUX
    public bool CanMove { get; private set; } = true;
    public bool CanAct  { get; private set; } = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
        stats = GetComponent<PlayerStats>();
    }


    void Update()
    {
        // ⛔ Le CharacterController est désactivé (transition, load…)
        if (!controller.enabled)
            return;

        ApplyGravity();

        if (CanMove && !isDashing)
            Move();
    }



    void ApplyGravity()
    {
        if (!controller.enabled)
            return;

        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime * 0.1f;
    }


    void Move()
    {
        float moveSpeed = stats.GetStat(StatType.MoveSpeed);

        Vector2 moveInput = input.MoveInput;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move.y = verticalVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    // ─── DASH ───────────────────────────────────────────────



    // ─── VERROU API ──────────────────────────────────────────

    public void SetMovement(bool value)
    {
        CanMove = value;
        if (!value)
            verticalVelocity = -2f;
    }

    public void SetActions(bool value)
    {
        CanAct = value;
    }
}
