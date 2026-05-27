// Scripts/Player/PlayerDash.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed    = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    [Header("IFrames")]
    [SerializeField] private GameObject damageHitbox;
    [SerializeField] private float      postDashIFrames = 0.15f;

    private int   normalLayer;
    private int   invincibleLayer;
    private float invincibleTimer;

    [Header("Style")]
    public float spinZSpeed = 720f;

    [Header("Rotation")]
    public float alignDuration = 0.06f;

    [Header("Aim Restore")]
    public float restoreAimDuration = 0.12f;

    CharacterController controller;
    PlayerCombat        combat;

    float     dashTime;
    float     nextDash;
    bool      isDashing;
    Vector3   dashDirection;
    Vector3   lockedAimDirection;

    float      alignTime;
    Quaternion alignStartRot;
    Quaternion alignTargetRot;
    bool       aligning;

    float      spinAngle;
    Quaternion restoreStartRot;
    Quaternion restoreTargetRot;
    float      restoreTime;
    bool       restoringAim;

    void Awake()
    {
        controller      = GetComponent<CharacterController>();
        combat          = GetComponent<PlayerCombat>();
        normalLayer     = damageHitbox.layer;
        invincibleLayer = LayerMask.NameToLayer("Invincible");
    }

    public bool CanDash => Time.time >= nextDash && !isDashing && !restoringAim;

    public void StartDash(Vector3 direction, Vector3 aimDirection)
    {
        if (!CanDash) return;

        dashDirection      = direction.normalized;
        lockedAimDirection = aimDirection.normalized;

        alignStartRot  = transform.rotation;
        alignTargetRot = Quaternion.LookRotation(dashDirection);
        alignTime      = 0f;
        aligning       = true;
        dashTime       = dashDuration;
        isDashing      = true;
        spinAngle      = 0f;
        nextDash       = Time.time + dashCooldown;

        SetInvincible(true);

        // ← Son au lancement du dash
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.PlayDash();
    }

    void Update()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                SetInvincible(false);
        }

        if (restoringAim)
        {
            restoreTime += Time.deltaTime;
            float t = restoreTime / restoreAimDuration;

            transform.rotation = Quaternion.Slerp(
                restoreStartRot, restoreTargetRot, t);

            if (t >= 1f) restoringAim = false;
            return;
        }

        if (!isDashing) return;

        dashTime -= Time.deltaTime;

        if (aligning)
        {
            alignTime += Time.deltaTime;
            float t = alignTime / alignDuration;

            transform.rotation = Quaternion.Slerp(
                alignStartRot, alignTargetRot, t);

            if (t >= 1f) aligning = false;
        }
        else
        {
            spinAngle += spinZSpeed * Time.deltaTime;
            Quaternion baseRot = Quaternion.LookRotation(dashDirection);
            Quaternion roll    = Quaternion.AngleAxis(spinAngle, Vector3.forward);
            transform.rotation = baseRot * roll;
        }

        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        if (dashTime <= 0f)
        {
            isDashing       = false;
            invincibleTimer = postDashIFrames;

            restoreStartRot  = transform.rotation;
            restoreTargetRot = Quaternion.LookRotation(lockedAimDirection);
            restoreTime      = 0f;

            if (combat.isShooting)
                restoringAim = true;
        }
    }

    void SetInvincible(bool value)
    {
        if (damageHitbox == null) return;
        damageHitbox.layer = value ? invincibleLayer : normalLayer;
    }
}