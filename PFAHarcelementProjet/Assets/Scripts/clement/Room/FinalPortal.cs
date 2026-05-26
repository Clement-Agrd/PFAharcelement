using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPortal : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        XPManager.Instance.ConvertGoldToXP();
        used = true;

        CleanupAndLoad();
    }

    void CleanupAndLoad()
    {
        // ✅ récupère tous les objets, même persistants
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // ✅ skip ceux qui sont dans un asset/prefab
            if (!obj.scene.IsValid()) continue;

            // ✅ skip GameManagers (tagged Persistent)
            if (obj.CompareTag("Persistent")) continue;

            // ✅ on ne détruit pas le portail lui-même
            if (obj == gameObject) continue;

            Destroy(obj);
        }

        SceneManager.LoadScene(menuSceneName);
    }
}