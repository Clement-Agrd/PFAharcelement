// Scripts/Player/PlayerCombat.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    enum AimDevice { Mouse, Gamepad }

    Vector3   lastMouseAimDirection = Vector3.forward;
    AimDevice activeAimDevice       = AimDevice.Mouse;
    Vector2   lastMousePos;
    float     lastMouseMoveTime;

    [Header("Mouse Aim")]
    public float mouseAimTimeout = 0.25f;

    [Header("Combat")]
    public Transform   firePoint;
    public GameObject  projectile;
    public PlayerMelee melee;
    public float       rotationSpeed = 15f;
    public PlayerDash  dash;
    public bool        isShooting = false;

    PlayerController   controller;
    PlayerInputHandler input;
    PlayerStats        stats;

    float nextFire;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        input      = GetComponent<PlayerInputHandler>();
        stats      = GetComponent<PlayerStats>();

        if (dash == null)
            dash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        DetectInputDevice();

        if (!controller.CanAct)                 return;
        if (melee != null && melee.IsAttacking) return;

        Vector3 aimDirection  = GetAimDirection();
        Vector3 moveDirection = GetMoveDirection();

        isShooting =
            input.ShootPressed ||
            (input.aimJoystick != null &&
             input.aimJoystick.Input.magnitude > 0.3f);

        if (isShooting)
        {
            RotateTowards(aimDirection);
            Shoot(aimDirection);
        }
        else if (input.AimInput.magnitude > 0.3f)
        {
            RotateTowards(aimDirection);
        }
        else if (moveDirection != Vector3.zero)
        {
            RotateTowards(moveDirection);
        }

        if (input.MeleePressed && melee != null)
            melee.TryAttack(aimDirection);

        if (input.DashPressed && dash != null && dash.CanDash)
        {
            Vector3 dashDir = moveDirection != Vector3.zero
                ? moveDirection
                : aimDirection;

            dash.StartDash(dashDir, aimDirection);
        }
    }

    void DetectInputDevice()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (mousePos != lastMousePos)
            {
                lastMousePos      = mousePos;
                lastMouseMoveTime = Time.time;
                activeAimDevice   = AimDevice.Mouse;
            }
        }

        if (input.AimInput.magnitude > 0.3f)
            activeAimDevice = AimDevice.Gamepad;
    }

    Vector3 GetMoveDirection()
    {
        Vector2 move = input.MoveInput;
        return move.magnitude > 0.1f
            ? new Vector3(move.x, 0f, move.y).normalized
            : Vector3.zero;
    }
    
    public void TriggerDash()
    {
        if (dash == null || !dash.CanDash) return;

        Vector3 moveDirection = GetMoveDirection();
        Vector3 aimDirection  = GetAimDirection();

        Vector3 dashDir = moveDirection != Vector3.zero
            ? moveDirection
            : aimDirection;

        dash.StartDash(dashDir, aimDirection);
    }
    
    Vector3 GetAimDirection()
    {
        if (activeAimDevice == AimDevice.Gamepad)
        {
            Vector2 aim = input.AimInput;
            if (aim.magnitude > 0.3f)
                return new Vector3(aim.x, 0f, aim.y).normalized;
            return transform.forward;
        }

        if (activeAimDevice == AimDevice.Mouse)
        {
            if (Time.time - lastMouseMoveTime < mouseAimTimeout)
            {
                Ray   ray    = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane ground = new Plane(Vector3.up, transform.position);

                if (ground.Raycast(ray, out float distance))
                {
                    Vector3 dir = ray.GetPoint(distance) - transform.position;
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.001f)
                        lastMouseAimDirection = dir.normalized;
                }
            }
            return lastMouseAimDirection;
        }

        return transform.forward;
    }

    void Shoot(Vector3 direction)
    {
        if (Time.time < nextFire) return;

        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float fireRate = (1f / attackSpeed) *
                         (1f - Mathf.Clamp01(cooldownReduction));

        direction.y = 0;
        direction.Normalize();

        GameObject proj = Instantiate(projectile, firePoint.position,
                                      Quaternion.LookRotation(direction));

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.damage         = stats.GetStat(StatType.RangedDamage);
            p.speed          = stats.GetStat(StatType.ProjectileSpeed);
            p.lifeStealRatio = stats.GetStat(StatType.LifeSteal);
        }

        nextFire = Time.time + fireRate;
    }

    void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, target, rotationSpeed * Time.deltaTime);
    }

    public void TriggerMelee()
    {
        if (melee == null) return;
        melee.TryAttack(GetAimDirection());
    }

    public void CancelCombat()
    {
        nextFire = 0f;
        if (melee != null) melee.CancelAttack();
    }

    void OnDisable()
    {
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.SetShootPressed(false);
    }
}