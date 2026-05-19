// Scripts/Enemies/EnemyStunned.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunned : MonoBehaviour
{
    private MonoBehaviour[] enemyScripts;

    public void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        enemyScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in enemyScripts)
        {
            if (script == this) continue;
            script.enabled = false;
        }

        Renderer[] renderers       = GetComponentsInChildren<Renderer>();
        List<Color> originalColors = new List<Color>();
        foreach (Renderer r in renderers)
        {
            originalColors.Add(r.material.color);
            r.material.color = Color.yellow;
        }

        yield return new WaitForSeconds(duration);

        foreach (MonoBehaviour script in enemyScripts)
        {
            if (script == this) continue;
            if (script != null) script.enabled = true;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].material.color = originalColors[i];
        }

        Destroy(this);
    }
}