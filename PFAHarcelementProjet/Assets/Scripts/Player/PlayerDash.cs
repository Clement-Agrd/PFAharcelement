using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    [Header("Style")]
    public float spinZSpeed = 720f; // degrés par seconde

    CharacterController controller;

    float dashTime;
    float nextDash;
    bool isDashing;
    Vector3 dashDirection;
    
    
    [Header("Rotation")]
    public float alignDuration = 0.06f; // temps de rotation douce au début
    float alignTime;
    Quaternion alignStartRot;
    Quaternion alignTargetRot;
    bool aligning;
    


    float spinAngle; // ✅ ACCUMULATEUR DE SPIN

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public bool CanDash => Time.time >= nextDash && !isDashing;

    public void StartDash(Vector3 direction)
    {
        if (!CanDash) return;

        dashDirection = direction.normalized;

        // 🔁 Prépare l'alignement smooth
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
        if (!isDashing) return;

        dashTime -= Time.deltaTime;

        // 🔄 ALIGNEMENT SMOOTH AU DÉBUT
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
            // 🔥 SPIN Z APRÈS ALIGNEMENT
            spinAngle += spinZSpeed * Time.deltaTime;

            Quaternion baseRot = Quaternion.LookRotation(dashDirection);
            Quaternion roll = Quaternion.AngleAxis(spinAngle, Vector3.forward);

            transform.rotation = baseRot * roll;
        }

        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        if (dashTime <= 0f)
        {
            isDashing = false;
            transform.rotation = Quaternion.LookRotation(dashDirection);
        }
    }
}
