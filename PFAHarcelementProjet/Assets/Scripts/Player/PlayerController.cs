// Scripts/Player/PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float gravity       = -20f;

    private CharacterController controller;
    private PlayerInputHandler  input;
    private PlayerStats         stats;
    private float               verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input      = GetComponent<PlayerInputHandler>();
        stats      = GetComponent<PlayerStats>();
    }

    void Update()
    {
        ApplyGravity();
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
        float moveSpeed = stats.GetStat(StatType.MoveSpeed); // ← vient de PlayerStats

        Vector2 moveInput = input.MoveInput;
        Vector3 move      = new Vector3(moveInput.x, 0, moveInput.y);

        // On injecte la gravité dans le déplacement vertical
        move.y = verticalVelocity;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void SetMovement(bool enabled)
    {
        verticalVelocity = -2f;
        this.enabled = enabled;
    }
}