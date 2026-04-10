using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectile;
    public float fireRate = 0.25f;

    PlayerInputHandler input;
    float nextFire;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        Vector3 direction = GetAimDirection();

        // 🔫 DISTANCE
        if (input.ShootPressed && direction != Vector3.zero)
        {
            Shoot(direction);
        }

        // 👊 MELEE
        if (input.MeleePressed)
        {
            Debug.Log("👊 Attaque mêlée");
        }
    }

    // 🎯 DIRECTION DE VISÉE
    Vector3 GetAimDirection()
    {
        // 🎮 Manette / Mobile
        Vector2 aim = input.AimInput;
        if (aim.magnitude > 0.3f)
        {
            return new Vector3(aim.x, 0, aim.y);
        }

        // 🖱️ Souris (top-down)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        if (Time.time < nextFire) return;

        direction.y = 0;
        direction.Normalize();

        firePoint.rotation = Quaternion.LookRotation(direction);
        Instantiate(projectile, firePoint.position, firePoint.rotation);

        nextFire = Time.time + fireRate;
    }
}
