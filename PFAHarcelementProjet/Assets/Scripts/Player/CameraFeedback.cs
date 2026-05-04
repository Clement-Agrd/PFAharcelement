using UnityEngine;

public class CameraFeedback : MonoBehaviour
{
    CameraShake cameraShake;
    DamageFlash damageFlash;

    void Awake()
    {
        cameraShake = GetComponent<CameraShake>();
        damageFlash = FindObjectOfType<DamageFlash>();
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += PlayFeedback;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= PlayFeedback;
    }

    void PlayFeedback()
    {
        cameraShake?.Shake(0.15f, 0.2f);
        damageFlash?.Flash();
    }
}