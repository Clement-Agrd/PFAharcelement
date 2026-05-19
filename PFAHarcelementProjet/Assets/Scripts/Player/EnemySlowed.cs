// Scripts/Enemies/EnemySlowed.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlowed : MonoBehaviour
{
    public void Slow(float slowness, float duration)
    {
        StartCoroutine(SlowCoroutine(slowness, duration));
    }

    IEnumerator SlowCoroutine(float slowness, float duration)
    {
        UnityEngine.AI.NavMeshAgent agent =
            GetComponent<UnityEngine.AI.NavMeshAgent>();

        float originalSpeed = 0f;
        if (agent != null)
        {
            originalSpeed = agent.speed;
            agent.speed  *= (1f - slowness);
        }

        Renderer[] renderers       = GetComponentsInChildren<Renderer>();
        List<Color> originalColors = new List<Color>();
        foreach (Renderer r in renderers)
        {
            originalColors.Add(r.material.color);
            r.material.color = new Color(0.5f, 0f, 0.5f, 1f);
        }

        yield return new WaitForSeconds(duration);

        if (agent != null)
            agent.speed = originalSpeed;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].material.color = originalColors[i];
        }

        Destroy(this);
    }
}