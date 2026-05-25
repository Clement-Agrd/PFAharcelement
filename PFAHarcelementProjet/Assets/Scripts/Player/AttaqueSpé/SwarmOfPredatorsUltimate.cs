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
    private float          swarmDuration;
    private float          baseCooldown;
    private GameObject     swarmPrefab;
    private GameObject     projectilePrefab;
    private float          lastUseTime  = -99f;
    private bool           isActive     = false;
    private PlayerControls controls;
    private PlayerStats    stats;

    private List<GameObject> activeAllies = new List<GameObject>();

    public bool  IsActive           => isActive;
    public float GetFinalCooldown() => baseCooldown * (1f - Mathf.Clamp01(stats.GetStat(StatType.CooldownReduction)));
    public float GetRemaining()     => Mathf.Max(0f, GetFinalCooldown() - (Time.time - lastUseTime));
    public float GetCooldownRatio() => GetFinalCooldown() > 0 ? GetRemaining() / GetFinalCooldown() : 0f;
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
        Debug.Log("🔓 Meute débloquée");
        UltimateUI ui = FindObjectOfType<UltimateUI>();
        if (ui != null) ui.OnUnlock();
    }

    public void Setup(int count, float damage, float duration,
                      float cooldown, GameObject prefab)
    {
        swarmCount    = count;
        swarmDuration = duration;
        baseCooldown  = cooldown;
        swarmPrefab   = prefab;

        // Récupère le projectile du joueur
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat != null)
            projectilePrefab = combat.projectile;
        else
            Debug.LogWarning("⚠️ PlayerCombat introuvable — projectile non assigné");
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
        activeAllies.Clear();

        for (int i = 0; i < swarmCount; i++)
        {
            if (swarmPrefab == null) continue;

            float   side     = (i % 2 == 0) ? -1f : 1f;
            float   offset   = Mathf.Ceil((i + 1) / 2f) * 2.5f;
            Vector3 spawnPos = transform.position +
                               transform.right * side * offset;

            GameObject ally = Instantiate(
                swarmPrefab,
                spawnPos,
                transform.rotation
            );

            SwarmAlly allyScript = ally.GetComponent<SwarmAlly>();
            if (allyScript != null)
                allyScript.Setup(
                    transform,
                    swarmDuration,
                    i,
                    swarmCount,
                    projectilePrefab,
                    stats          // ← passe les stats du joueur directement
                );

            activeAllies.Add(ally);
        }

        Debug.Log($"🦈 Meute : {swarmCount} alliés en ligne");
        Debug.Log($"🦈 RangedDamage : {stats.GetStat(StatType.RangedDamage):F1}");
        Debug.Log($"🦈 ProjectileSpeed : {stats.GetStat(StatType.ProjectileSpeed):F1}");
        Debug.Log($"🦈 AttackSpeed : {stats.GetStat(StatType.AttackSpeed):F1}");

        yield return new WaitForSeconds(swarmDuration);

        foreach (GameObject ally in activeAllies)
            if (ally != null) Destroy(ally);

        activeAllies.Clear();
        isActive = false;
        Debug.Log("🦈 Meute terminée");
    }
}