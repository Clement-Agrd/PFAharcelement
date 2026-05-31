// CameraFeedback.cs
using UnityEngine;

public class CameraFeedback : MonoBehaviour
{
    CameraShake cameraShake;

    // Cache local — ne dépend plus du singleton statique
    private DamageFlash cachedDamageFlash = null;

    void Awake()
    {
        cameraShake = GetComponent<CameraShake>();
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += PlayFeedback;
        cachedDamageFlash = null; // force la recherche au prochain appel
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= PlayFeedback;
    }

    DamageFlash GetDamageFlash()
    {
        if (cachedDamageFlash != null)
            return cachedDamageFlash;

        if (DamageFlash.Instance != null)
        {
            cachedDamageFlash = DamageFlash.Instance;
            return cachedDamageFlash;
        }

        cachedDamageFlash = FindFirstObjectByType<DamageFlash>();
        return cachedDamageFlash;
    }

    void PlayFeedback()
    {
        cameraShake?.Shake(0.15f, 0.2f);

        DamageFlash flash = GetDamageFlash();
        if (flash != null)
            flash.Flash();
        else
            Debug.LogWarning("[CameraFeedback] DamageFlash introuvable");
    }
}