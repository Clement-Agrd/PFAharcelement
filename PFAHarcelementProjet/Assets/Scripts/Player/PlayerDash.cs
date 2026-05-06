using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    [Header("Style")]
    public float spinZSpeed = 720f;

    [Header("Rotation")]
    public float alignDuration = 0.06f;

    [Header("Aim Restore")]
    public float restoreAimDuration = 0.12f;

    CharacterController controller;
    PlayerCombat combat;

    float dashTime;
    float nextDash;
    bool isDashing;

    Vector3 dashDirection;
    Vector3 lockedAimDirection;

    // Alignement début dash
    float alignTime;
    Quaternion alignStartRot;
    Quaternion alignTargetRot;
    bool aligning;

    // Spin
    float spinAngle;

    // Restauration visée
    Quaternion restoreStartRot;
    Quaternion restoreTargetRot;
    float restoreTime;
    bool restoringAim;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        combat = GetComponent<PlayerCombat>();
    }

    public bool CanDash => Time.time >= nextDash && !isDashing && !restoringAim;

    public void StartDash(Vector3 direction, Vector3 aimDirection)
    {
        if (!CanDash) return;

        dashDirection = direction.normalized;
        lockedAimDirection = aimDirection.normalized;

        // Alignement vers la direction du dash
        alignStartRot  = transform.rotation;
        alignTargetRot = Quaternion.LookRotation(dashDirection);
        alignTime = 0f;
        aligning = true;

        dashTime = dashDuration;
        isDashing = true;
        spinAngle = 0f;
        nextDash = Time.time + dashCooldown;
    }

    void Update()
    {
        // 🔁 RESTAURATION DE LA VISÉE (SMOOTH)
        if (restoringAim)
        {
            restoreTime += Time.deltaTime;
            float t = restoreTime / restoreAimDuration;

            transform.rotation = Quaternion.Slerp(
                restoreStartRot,
                restoreTargetRot,
                t
            );

            if (t >= 1f)
                restoringAim = false;

            return;
        }

        if (!isDashing) return;

        dashTime -= Time.deltaTime;

        // 🔄 ALIGN AU DÉBUT
        if (aligning)
        {
            alignTime += Time.deltaTime;
            float t = alignTime / alignDuration;

            transform.rotation = Quaternion.Slerp(
                alignStartRot,
                alignTargetRot,
                t
            );

            if (t >= 1f)
                aligning = false;
        }
        else
        {
            // 🔥 SPIN
            spinAngle += spinZSpeed * Time.deltaTime;

            Quaternion baseRot = Quaternion.LookRotation(dashDirection);
            Quaternion roll = Quaternion.AngleAxis(spinAngle, Vector3.forward);
            transform.rotation = baseRot * roll;
        }

        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        // ✅ FIN DU DASH → RESTORE AIM
        if (dashTime <= 0f && combat.isShooting)
        {
            isDashing = false;

            restoreStartRot  = transform.rotation;
            restoreTargetRot = Quaternion.LookRotation(lockedAimDirection);

            restoreTime = 0f;
            restoringAim = true;
        }
        else if(dashTime <= 0f)
        {
            isDashing = false;

            restoreStartRot  = transform.rotation;
            restoreTargetRot = Quaternion.LookRotation(lockedAimDirection);

            restoreTime = 0f;
        }
    }
}