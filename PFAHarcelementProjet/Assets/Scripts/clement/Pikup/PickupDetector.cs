// PickupDetector.cs
using System.Collections.Generic;
using UnityEngine;

public class PickupDetector : MonoBehaviour
{
    public LayerMask pickupLayer;
    public float detectionRadius = 4f;

    private readonly List<RewardPickup> nearbyPickups = new();
    private RewardPickup closestPickup;

    private int lastPickupId = -1;
    private RewardPickup lastPreviewedPickup = null;

    // Cache local — ne dépend plus du singleton statique
    private UIStatsPanel cachedStatsPanel = null;

    void Update()
    {
        FindPickups();
        UpdateClosestPickup();
    }

    // Cherche le panel dans la scène si le cache est vide ou détruit
    UIStatsPanel GetStatsPanel()
    {
        if (cachedStatsPanel != null)
            return cachedStatsPanel;

        // Cherche d'abord via le singleton
        if (UIStatsPanel.Instance != null)
        {
            cachedStatsPanel = UIStatsPanel.Instance;
            Debug.Log("[PickupDetector] UIStatsPanel trouvé via Instance");
            return cachedStatsPanel;
        }

        // Fallback : cherche dans la scène active
        cachedStatsPanel = FindFirstObjectByType<UIStatsPanel>();

        if (cachedStatsPanel != null)
            Debug.Log("[PickupDetector] UIStatsPanel trouvé via FindFirstObjectByType");
        
        return cachedStatsPanel;
    }

    void FindPickups()
    {
        nearbyPickups.Clear();

        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionRadius, pickupLayer);

        foreach (var hit in hits)
        {
            RewardPickup pickup = hit.GetComponent<RewardPickup>();
            if (pickup == null) continue;
            if (pickup.gameObject == null) continue;
            nearbyPickups.Add(pickup);
        }
    }

    void UpdateClosestPickup()
    {
        float minDist = float.MaxValue;
        closestPickup = null;

        foreach (var pickup in nearbyPickups)
        {
            if (pickup == null) continue;
            float dist = Vector3.Distance(transform.position, pickup.transform.position);
            if (dist <= pickup.interactionRange && dist < minDist)
            {
                minDist = dist;
                closestPickup = pickup;
            }
        }

        int currentId = closestPickup != null ? closestPickup.GetInstanceID() : -1;

        // ───── BuffUI ─────────────────────────────────────────────────────────
        if (currentId != lastPickupId)
        {
            lastPickupId = currentId;

            if (BuffUI.Instance != null)
                BuffUI.Instance.SetPickup(closestPickup);
        }

        // ───── UIStatsPanel preview ───────────────────────────────────────────
        UIStatsPanel panel = GetStatsPanel();

        if (panel == null)
            return;

        if (closestPickup == lastPreviewedPickup)
            return;

        lastPreviewedPickup = closestPickup;

        if (closestPickup != null)
        {
            Debug.Log($"[PickupDetector] PreviewBuff → {closestPickup.name}");
            panel.PreviewBuff(closestPickup.buffData);
        }
        else
        {
            Debug.Log("[PickupDetector] ClearPreview");
            panel.ClearPreview();
        }
    }

    void OnEnable()
    {
        RewardPickup.OnPickupConsumed += HandlePickupConsumed;
        ResetState();
    }

    void OnDisable()
    {
        RewardPickup.OnPickupConsumed -= HandlePickupConsumed;
    }

    // Vide le cache quand la scène change pour forcer la recherche au prochain Update
    void OnDestroy()
    {
        cachedStatsPanel = null;
    }

    void HandlePickupConsumed(RewardPickup pickup)
    {
        ResetState();

        if (BuffUI.Instance != null)
            BuffUI.Instance.SetPickup(null);

        UIStatsPanel panel = GetStatsPanel();
        if (panel != null)
            panel.ClearPreview();
    }

    public void ForceRefresh()
    {
        cachedStatsPanel = null; // force la recherche au prochain Update
        ResetState();

        if (BuffUI.Instance != null)
            BuffUI.Instance.SetPickup(null);

        UIStatsPanel panel = GetStatsPanel();
        if (panel != null)
            panel.ClearPreview();
    }

    void ResetState()
    {
        closestPickup       = null;
        lastPickupId        = -1;
        lastPreviewedPickup = null;
    }
}