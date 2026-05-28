using UnityEngine;

public class RunInitializer : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("❌ Player introuvable en début de run");
            return;
        }

        player.SetActive(true);
        
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetMovement(true);
            controller.SetActions(true);
        }

        Debug.Log("🎮 Run démarrée - Player activé");
    }
}