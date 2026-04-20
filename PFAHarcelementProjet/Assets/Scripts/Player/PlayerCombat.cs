// Scripts/Player/PlayerCombat.cs
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform  firePoint;
    public GameObject projectile;

    private PlayerInputHandler input;
    private PlayerStats        stats;
    private float              nextFire;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        Vector3 direction = GetAimDirection();

        // MOBILE : joystick droit = vise + tire automatiquement
        bool mobileAutoShoot = input.aimJoystick != null && input.aimJoystick.Input.magnitude > 0.3f;

        if (direction != Vector3.zero)
        {
            if (input.ShootPressed || mobileAutoShoot)
                Shoot(direction);
        }

        if (input.MeleePressed)
            Debug.Log("👊 Attaque mêlée");
    }

    // ─── DIRECTION DE VISÉE ──────────────────────────────────────────────────

    Vector3 GetAimDirection()
    {
        // 🎮 Manette / Mobile
        Vector2 aim = input.AimInput;
        if (aim.magnitude > 0.3f)
            return new Vector3(aim.x, 0, aim.y);

        // 🖱️ Souris (top-down)
        Ray   ray    = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return (point - transform.position).normalized;
        }

        return Vector3.zero;
    }

    // ─── TIR ─────────────────────────────────────────────────────────────────

    void Shoot(Vector3 direction)
    {
        // Cooldown basé sur AttackSpeed + CooldownReduction
        float attackSpeed       = stats.GetStat(StatType.AttackSpeed);
        float cooldownReduction = stats.GetStat(StatType.CooldownReduction);
        float fireRate          = (1f / attackSpeed) * (1f - Mathf.Clamp01(cooldownReduction));

        if (Time.time < nextFire) return;

        direction.y = 0;
        direction.Normalize();

        firePoint.rotation = Quaternion.LookRotation(direction);

        // Instanciation + injection des stats dans le projectile
        GameObject proj = Instantiate(projectile, firePoint.position, firePoint.rotation);

        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.damage         = stats.GetStat(StatType.Damage);
            projScript.speed          = stats.GetStat(StatType.ProjectileSpeed);
            projScript.lifeStealRatio = stats.GetStat(StatType.LifeSteal);
        }

        nextFire = Time.time + fireRate;
    }
}