using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private float nextFireTime;
    private PlayerInputManager input;

    void Start()
    {
        input = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        if (input.Aim.magnitude > 0.5f)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}