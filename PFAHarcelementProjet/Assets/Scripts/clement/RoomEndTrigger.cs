using UnityEngine;

public class RoomEndTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Vérifie si le joueur entre dans la zone
        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Charge la salle suivante
            StageManager.Instance.LoadNext();
        }
    }
}