// Scripts/Player/PlayerDash.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    CharacterController controller;

    float dashTime;
    float nextDash;
    bool isDashing;
    Vector3 dashDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public bool CanDash => Time.time >= nextDash && !isDashing;

    public void StartDash(Vector3 direction)
    {
        if (!CanDash) return;

        dashDirection = direction.normalized;
        dashTime = dashDuration;
        isDashing = true;

        nextDash = Time.time + dashCooldown;
    }

    void Update()
    {
        if (!isDashing) return;

        dashTime -= Time.deltaTime;

        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        if (dashTime <= 0f)
        {
            isDashing = false;
        }
    }
}