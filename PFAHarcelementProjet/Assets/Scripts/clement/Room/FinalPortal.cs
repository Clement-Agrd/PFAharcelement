// Scripts/FinalPortal.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPortal : MonoBehaviour
{
    [Header("Scène")]
    public string menuSceneName = "MainMenu";

    private bool used = false;

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        // Convertit le gold en XP
        if (XPManager.Instance != null)
            XPManager.Instance.ConvertGoldToXP();

        ResetRun(other.gameObject);

        // Lance le slideshow puis retour menu
        if (EndSlideshowUI.Instance != null)
            EndSlideshowUI.Instance.Play(GoToMenu);
        else
            GoToMenu();
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    void ResetRun(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ClearAllModifiers();
            stats.ResetRunStats();
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.ResetPlayer();

        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);

        player.SetActive(false);
        player.transform.position = Vector3.zero;
    }
}