using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStagePortal : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName;

    [Header("Settings")]
    public float interactionDelay = 0.2f; // évite le trigger instantané

    private bool canUse = false;
    private bool used = false;

    void Start()
    {
        // ✅ petit délai pour éviter activation immédiate
        Invoke(nameof(EnablePortal), interactionDelay);
    }

    void EnablePortal()
    {
        canUse = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used || !canUse)
            return;

        if (!other.CompareTag("Player"))
            return;

        used = true;

        LoadNextStage(other.gameObject);
    }

    void LoadNextStage(GameObject player)
    {
        // ✅ Optionnel : freeze joueur
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetMovement(false);
            controller.SetActions(false);
        }

        // ✅ Optionnel : cancel combat
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.CancelCombat();
        }

        // ✅ changement de scène
        SceneManager.LoadScene(nextSceneName);
    }
}