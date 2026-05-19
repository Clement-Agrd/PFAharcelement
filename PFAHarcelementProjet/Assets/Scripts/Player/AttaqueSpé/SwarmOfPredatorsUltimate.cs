// Scripts/Ultimates/SwarmOfPredatorsUltimate.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwarmOfPredatorsUltimate : MonoBehaviour
{
    [Header("Déblocage")]
    public bool startUnlocked = false;

    public bool IsUnlocked { get; private set; } = false;

    private int            swarmCount;
    private float          swarmDamage;
    private float          swarmDuration;
    private float          swarmCooldown;
    private GameObject     swarmPrefab;
    private float          lastUseTime = -99f;
    private bool           isActive    = false;
    private PlayerControls controls;
    private PlayerStats    stats;

    private List<GameObject> activeAlly = new List<GameObject>();

    public bool  IsActive           => isActive;
    public float GetRemaining()     => Mathf.Max(0f, swarmCooldown - (Time.time - lastUseTime));
    public float GetCooldownRatio() => swarmCooldown > 0 ? GetRemaining() / swarmCooldown : 0f;
    public bool  IsReady()          => IsUnlocked && GetRemaining() <= 0f && !isActive;

    void Awake()
    {
        controls = new PlayerControls();
        stats    = GetComponent<PlayerStats>();
        if (startUnlocked) Unlock();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.SpecialAttack.performed += OnInput;
    }

    void OnDisable()
    {
        controls.Player.SpecialAttack.performed -= OnInput;
        controls.Player.Disable();
    }

    public void Unlock()
    {
        IsUnlocked = true;
        Debug.Log("🔓 Essaim débloqué");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(int count, float damage, float duration,
                      float cooldown, GameObject prefab)
    {
        swarmCount    = count;
        swarmDamage   = damage;
        swarmDuration = duration;
        swarmCooldown = cooldown;
        swarmPrefab   = prefab;
    }

    void OnInput(InputAction.CallbackContext ctx)
    {
        if (!IsUnlocked) return;
        if (IsReady()) Activate();
    }

    public void Activate()
    {
        if (!IsReady()) return;
        StartCoroutine(SwarmCoroutine());
        lastUseTime = Time.time;

        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnActivate();
    }

    IEnumerator SwarmCoroutine()
    {
        isActive = true;
        activeAlly.Clear();

        float damage = swarmDamage + stats.GetStat(StatType.MeleeDamage) * 0.5f;

        // Spawne les requins alliés en cercle autour du joueur
        for (int i = 0; i < swarmCount; i++)
        {
            float angle  = i * (360f / swarmCount) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * 3f,
                0f,
                Mathf.Sin(angle) * 3f
            );

            if (swarmPrefab != null)
            {
                GameObject ally = Instantiate(
                    swarmPrefab,
                    transform.position + offset,
                    Quaternion.identity
                );

                SwarmAlly allyScript = ally.GetComponent<SwarmAlly>();
                if (allyScript != null)
                    allyScript.Setup(transform, damage, swarmDuration);

                activeAlly.Add(ally);
            }
        }

        Debug.Log($"🦈 Essaim actif : {swarmCount} alliés pendant {swarmDuration}s");

        yield return new WaitForSeconds(swarmDuration);

        foreach (GameObject ally in activeAlly)
            if (ally != null) Destroy(ally);

        activeAlly.Clear();
        isActive = false;
        Debug.Log("🦈 Essaim terminé");
    }
}