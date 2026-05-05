// Scripts/Player/SpecialAttack.cs
using System.Collections;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    [Header("Explosion")]
    public GameObject explosionVFX;
    public float      explosionRadius = 5f;
    public float      baseCooldown    = 8f;

    [Header("Visée")]
    public GameObject aimIndicator;  // glisse l'objet AimIndicator de la scène ici
    public float      maxRange = 15f;

    private PlayerStats        stats;
    private PlayerInputHandler input;
    private float              lastUseTime = -99f;
    private bool               isAiming    = false;
    private Vector3            aimPosition;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInputHandler>();

        // S'assure que l'indicateur est caché au départ
        if (aimIndicator != null)
            aimIndicator.SetActive(false);
    }

    void Update()
    {
        HandleAiming();
        HandleInput();
    }

    public float GetCooldownReduction()  => Mathf.Clamp01(stats.GetStat(StatType.CooldownReduction));
    public float GetFinalCooldown()      => baseCooldown * (1f - GetCooldownReduction());
    public float GetCooldownRemaining()  => Mathf.Max(0f, GetFinalCooldown() - (Time.time - lastUseTime));
    public float GetCooldownRatio()      => GetCooldownRemaining() / GetFinalCooldown();
    public bool  IsReady()               => GetCooldownRemaining() <= 0f;

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ToggleAim();

        if (isAiming && Input.GetMouseButtonDown(0))
            Launch();

        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            if (isAiming) Launch();
            else          ToggleAim();
        }

        if (isAiming && (Input.GetKeyDown(KeyCode.Escape) ||
                         Input.GetMouseButtonDown(1)))
            CancelAim();
    }

    void HandleAiming()
    {
        if (!isAiming) return;

        // 🖱️ Souris
        Ray   ray    = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float dist))
        {
            Vector3 point     = ray.GetPoint(dist);
            Vector3 direction = point - transform.position;

            if (direction.magnitude > maxRange)
                point = transform.position + direction.normalized * maxRange;

            aimPosition = point;

            // Met à jour la position de l'indicateur
            if (aimIndicator != null)
            {
                aimIndicator.transform.position = new Vector3(
                    aimPosition.x,
                    transform.position.y + 0.1f, // légèrement au-dessus du sol
                    aimPosition.z
                );
            }
        }

        // 🎮 Manette
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            Vector3 dir = new Vector3(h, 0, v).normalized;
            aimPosition = transform.position + dir * maxRange;

            if (aimIndicator != null)
            {
                aimIndicator.transform.position = new Vector3(
                    aimPosition.x,
                    transform.position.y + 0.1f,
                    aimPosition.z
                );
            }
        }
    }

    void ToggleAim()
    {
        if (!IsReady())
        {
            Debug.Log($"⏳ Recharge : {GetCooldownRemaining():F1}s");
            return;
        }

        isAiming = !isAiming;

        if (aimIndicator != null)
            aimIndicator.SetActive(isAiming);

        if (isAiming)
            aimPosition = transform.position + transform.forward * 3f;
    }

    void CancelAim()
    {
        isAiming = false;

        if (aimIndicator != null)
            aimIndicator.SetActive(false);
    }

    public void Launch()
    {
        if (!IsReady()) return;

        CancelAim();
        StartCoroutine(ExplodeCoroutine());
        lastUseTime = Time.time;
    }

    public void OnSpecialButtonPressed()
    {
        if (!isAiming) ToggleAim();
        else           Launch();
    }

    IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(0.15f);
        Explode();
    }

    void Explode()
    {
        float damage = stats.GetStat(StatType.RangedDamage) * 1.5f;

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, aimPosition, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        Collider[] hits = Physics.OverlapSphere(aimPosition, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            float distance = Vector3.Distance(aimPosition, hit.transform.position);
            float falloff  = 1f - Mathf.Clamp01(distance / explosionRadius);
            float finalDmg = damage * falloff;

            target.TakeDamage(finalDmg);

            float lifeSteal = stats.GetStat(StatType.LifeSteal);
            if (lifeSteal > 0f)
            {
                PlayerHealth ph = GetComponent<PlayerHealth>();
                if (ph != null) ph.Heal(finalDmg * lifeSteal);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(isAiming ? aimPosition : transform.position, explosionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}