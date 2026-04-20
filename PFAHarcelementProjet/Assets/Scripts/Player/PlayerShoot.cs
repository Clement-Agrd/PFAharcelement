using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectile;
    public Transform firePoint;

    public float fireRate = 0.25f;

    private PlayerInputHandler input;

    float nextFire;

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }


    
   
    void Update()
    {
        Vector3 shootDirection = Vector3.zero;

        // 🎮 MANETTE / JOYSTICK
        Vector2 aimInput = input.AimInput;
        if (aimInput.magnitude > 0.5f)
        {
            shootDirection = new Vector3(aimInput.x, 0f, aimInput.y);
        }


        if (shootDirection != Vector3.zero)
        {
            Shoot(shootDirection);
        }
    }

    

    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return (point - transform.position).normalized;
        }

        return transform.forward;
    }

   
   
    void Shoot(Vector3 direction)
    {
        if (Time.time < nextFire) return;

        direction.y = 0f;
        direction.Normalize();

        firePoint.rotation = Quaternion.LookRotation(direction);
        Instantiate(projectile, firePoint.position, firePoint.rotation);

        nextFire = Time.time + fireRate;
    }


}