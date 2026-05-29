// Scripts/NPC/NPCDialogue.cs

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NPCDialogue : MonoBehaviour
{
    [Header("Détection joueur")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Dialogue")]
    public Sprite npcPortrait;
    public string npcName = "Entité des Abysses";
    public List<string> dialogLines = new List<string>();

    private bool playerInRange = false;
    private bool dialogDone = false;
    private bool dialogOpen = false;

    void Start()
    {
        if (dialogLines == null || dialogLines.Count == 0)
            Debug.LogWarning("⚠️ NPCDialogue : aucune ligne de dialogue");
        else
            Debug.Log($"✅ NPCDialogue prêt avec {dialogLines.Count} lignes");
    }

    void Update()
    {
        if (!playerInRange) return;
        if (dialogDone) return;
        if (dialogOpen) return;

        bool pressE = Input.GetKeyDown(interactKey);

        bool pressGamepad =
            Gamepad.current != null &&
            Gamepad.current.buttonNorth.wasPressedThisFrame;

        if (pressE || pressGamepad)
        {
            Debug.Log("🗣️ Interaction déclenchée");
            OpenDialogue();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🔵 Trigger entré : {other.name} | tag : {other.tag}");

        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (dialogDone)
            return;

        NPCInteractUI ui = FindObjectOfType<NPCInteractUI>(true);

        if (ui != null)
            ui.Show(interactKey.ToString());
        else
            Debug.LogError("❌ NPCInteractUI introuvable");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        NPCInteractUI ui = FindObjectOfType<NPCInteractUI>(true);

        if (ui != null)
            ui.Hide();
    }

    void OpenDialogue()
    {
        dialogOpen = true;

        // Mute sons joueur
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.Mute();

        NPCInteractUI interactUI = FindObjectOfType<NPCInteractUI>(true);

        if (interactUI != null)
            interactUI.Hide();

        DialogueUI ui = FindObjectOfType<DialogueUI>(true);

        if (ui == null)
        {
            Debug.LogError("❌ DialogueUI introuvable");
            dialogOpen = false;
            return;
        }

        ui.StartDialogue(
            npcPortrait,
            npcName,
            dialogLines,
            OnDialogueFinished);
    }

    void OnDialogueFinished()
    {
        dialogDone = true;
        dialogOpen = false;

        // Réactive les sons
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.Unmute();

        Debug.Log("✅ Dialogue terminé — ouverture choix ultime");

        UltimateChoiceUI choiceUI =
            FindObjectOfType<UltimateChoiceUI>(true);

        if (choiceUI != null)
            choiceUI.Show(UltimateManager.Instance.GetRandomChoices(3));
        else
            Debug.LogError("❌ UltimateChoiceUI introuvable");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}