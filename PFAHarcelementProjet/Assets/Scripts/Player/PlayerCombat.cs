// Scripts/Player/PlayerCombat.cs
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform  firePoint;
    public GameObject projectile;
    public float fireRate = 0.25f;
    public PlayerMelee melee;
    public float rotationSpeed = 15f;
    
    Quaternion targetRotation;

    private PlayerInputHandler input;
    private PlayerStats        stats;
    private float              nextFire;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        stats = GetComponent<PlayerStats>();
    }
    
    Vector3 GetMoveDirection()
    {
        Vector2 move = input.MoveInput;

        if (move.magnitude > 0.1f)
            return new Vector3(move.x, 0f, move.y).normalized;

        return Vector3.zero;
    }

    void Update()
    {
        Vector3 aimDirection  = GetAimDirection();
        Vector3 moveDirection = GetMoveDirection();

        bool mobileAutoShoot =
            input.aimJoystick != null &&
            input.aimJoystick.Input.magnitude > 0.3f;

        bool isShooting = input.ShootPressed || mobileAutoShoot;

        // ROTATION
        if (isShooting && aimDirection != Vector3.zero)
        {
            RotateTowards(aimDirection);
            Shoot(aimDirection);
        }
        else if (!isShooting && moveDirection != Vector3.zero)
        {
            RotateTowards(moveDirection);
        }

        // Mêlée
        if (input.MeleePressed && melee != null)
            melee.TryAttack();
    }

    Vector3 GetAimDirection()
    {
        Vector2 aim = input.AimInput;
        if (aim.magnitude > 0.3f)
            return new Vector3(aim.x, 0, aim.y);

        Ray   ray    = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return (point - transform.position).normalized;
        }

        return Vector3.zero;
    }

    void Shoot(Vector3 direction)
    {
        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float fireRate          = (1f / attackSpeed) * (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time < nextFire) return;

        direction.y = 0;
        direction.Normalize();

        Quaternion shotRotation = Quaternion.LookRotation(direction);

        GameObject proj = Instantiate(projectile, firePoint.position, shotRotation);

        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.damage         = stats.GetStat(StatType.RangedDamage); // ← RangedDamage
            projScript.speed          = stats.GetStat(StatType.ProjectileSpeed);
            projScript.lifeStealRatio = stats.GetStat(StatType.LifeSteal);
        }

        nextFire = Time.time + fireRate;
    }

    void RotateTowards(Vector3 direction)
    {
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}