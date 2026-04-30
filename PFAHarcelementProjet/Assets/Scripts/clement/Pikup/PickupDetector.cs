using System.Collections.Generic;
using UnityEngine;

public class PickupDetector : MonoBehaviour
{
    public LayerMask pickupLayer;
    public float detectionRadius = 4f;

    private readonly List<RewardPickup> nearbyPickups = new();
    private RewardPickup closestPickup;
    private RewardPickup lastPickup;


    void Update()
    {
        FindPickups();
        UpdateClosestPickup();
    }

    void FindPickups()
    {
        nearbyPickups.Clear();

        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectionRadius, pickupLayer);

        foreach (var hit in hits)
        {
            RewardPickup pickup = hit.GetComponent<RewardPickup>();
            if (pickup != null)
                nearbyPickups.Add(pickup);
        }
    }

    void UpdateClosestPickup()
    {
        float minDist = float.MaxValue;
        closestPickup = null;

        foreach (var pickup in nearbyPickups)
        {
            float dist = Vector3.Distance(transform.position, pickup.transform.position);

            if (dist <= pickup.interactionRange && dist < minDist)
            {
                minDist = dist;
                closestPickup = pickup;
            }
        }

        // 🔁 Si le pickup n’a pas changé → on ne fait rien
        if (closestPickup == lastPickup)
            return;

        lastPickup = closestPickup;

        // 📦 UI description
        BuffUI.Instance.SetPickup(closestPickup);

        // 📊 Preview stats
        if (closestPickup != null)
            UIStatsPanel.Instance.PreviewBuff(closestPickup.buffData);
        else
            UIStatsPanel.Instance.ClearPreview();
    }

    void OnEnable()
    {
        RewardPickup.OnPickupConsumed += HandlePickupConsumed;
    }

    void OnDisable()
    {
        RewardPickup.OnPickupConsumed -= HandlePickupConsumed;
    }

    void HandlePickupConsumed(RewardPickup pickup)
    {
        if (pickup == lastPickup)
        {
            lastPickup = null;
            closestPickup = null;

            BuffUI.Instance.SetPickup(null);
            UIStatsPanel.Instance.ClearPreview();

        }
    }
}