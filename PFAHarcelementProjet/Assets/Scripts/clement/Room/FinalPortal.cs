using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPortal : MonoBehaviour
{
    [Header("Scene")]
    public string menuSceneName = "MainMenu";

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        CleanupAndLoad();
    }

    void CleanupAndLoad()
    {
        // ✅ Garde une référence de ton manager
        XPManager xpManager = FindObjectOfType<XPManager>();

        // ✅ On récupère TOUS les objets de la scène
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // ❌ On ne détruit PAS le XPManager
            if (xpManager != null && obj == xpManager.gameObject)
                continue;

            // ❌ On ne détruit PAS le portail lui-même (optionnel)
            if (obj == this.gameObject)
                continue;

            // ✅ On détruit tout le reste
            Destroy(obj);
        }

        // ✅ On garde le XPManager entre les scènes
        if (xpManager != null)
        {
            DontDestroyOnLoad(xpManager.gameObject);
        }

        // ✅ On charge le menu
        SceneManager.LoadScene(menuSceneName);
    }
}