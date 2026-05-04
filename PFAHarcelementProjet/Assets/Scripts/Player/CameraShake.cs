using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    Vector3 originalPos;
    Coroutine shakeRoutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float intensity, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            Vector3 offset = Random.insideUnitSphere * intensity;
            transform.localPosition = originalPos + offset;

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
