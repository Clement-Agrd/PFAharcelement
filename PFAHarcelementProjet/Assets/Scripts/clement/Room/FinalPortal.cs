using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPortal : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (!other.CompareTag("Player"))
            return;

        used = true;

        // Convertit le gold en XP
        if (XPManager.Instance != null)
            XPManager.Instance.ConvertGoldToXP();

        ResetRun(other.gameObject);

        SceneManager.LoadScene(menuSceneName);
    }

    void ResetRun(GameObject player)
    {
        // Reset buffs
        PlayerStats stats = player.GetComponent<PlayerStats>();

        if (stats != null)
            stats.ClearAllModifiers();

        // Reset HP
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null)
            health.ResetPlayer();

        // Reset ultimate
        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);

        stats.ResetRunStats();

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);

        // Désactive le player dans le menu
        player.SetActive(false);

        // Reset position
        player.transform.position = Vector3.zero;
    }
}