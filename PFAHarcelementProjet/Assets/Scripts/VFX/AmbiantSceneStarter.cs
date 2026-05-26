// Scripts/Audio/AmbientSceneStarter.cs
using UnityEngine;

public class AmbientSceneStarter : MonoBehaviour
{
    void Start()
    {
        if (AmbientManager.Instance != null)
            AmbientManager.Instance.PlayAmbient();
        else
            Debug.LogError("❌ AmbientManager introuvable");
    }

    void OnDestroy()
    {
        // Arrête le son ambiant quand on quitte la scène
        if (AmbientManager.Instance != null)
            AmbientManager.Instance.StopAmbient();
    }
}