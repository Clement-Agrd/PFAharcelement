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
        Vector2 aimInput = input.AimInput;

        if(aimInput.magnitude > 0.5f)
        {
            Shoot(aimInput);
        }
    }

    void Shoot(Vector2 aimInput)
    {
        if(Time.time < nextFire) return;

        Vector3 direction = new Vector3(aimInput.x,0,aimInput.y);

        firePoint.rotation = Quaternion.LookRotation(direction);

        Instantiate(projectile, firePoint.position, firePoint.rotation);

        nextFire = Time.time + fireRate;
    }
}