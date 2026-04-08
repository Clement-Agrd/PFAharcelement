using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed     = 6f;
    public float rotationSpeed = 15f;
    public float gravity       = -20f;

    private CharacterController controller;
    private PlayerInputHandler  input;
    private float               verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input      = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        ApplyGravity();
        Move();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            verticalVelocity = -2f; // petite valeur négative pour coller au sol
        else
            verticalVelocity += gravity * Time.deltaTime * 0.1f;
    }

    void Move()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 move      = new Vector3(moveInput.x, 0, moveInput.y);

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // On injecte la gravité dans le déplacement vertical
        move.y = verticalVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
    public void SetMovement(bool enabled)
    {
        verticalVelocity = -2f; // reset avant de réactiver
        this.enabled = enabled;
    }
}