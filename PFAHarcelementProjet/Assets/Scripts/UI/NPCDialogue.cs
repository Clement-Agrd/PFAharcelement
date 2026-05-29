// Scripts/NPC/NPCDialogue.cs

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NPCDialogue : MonoBehaviour
{
    [Header("Détection joueur")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Dialogue")]
    [SerializeField] private Sprite npcPortrait;
    [SerializeField] private string npcName = "Entité des Abysses";
    [SerializeField] private List<string> dialogLines = new List<string>();

    [Header("Récompense")]
    [SerializeField] private bool giveUltimate = false;

    // États internes
    private bool playerInRange = false;

    // Dialogue déjà consommé définitivement
    private bool dialogueCompleted = false;

    // Dialogue actuellement ouvert
    private bool dialogueOpen = false;

    // Sécurité anti double lancement
    private bool interactionLocked = false;

    // Sécurité anti double callback
    private bool finishTriggered = false;

    // Cache UI
    private NPCInteractUI interactUI;
    private DialogueUI dialogueUI;
    private UltimateChoiceUI ultimateChoiceUI;

    // --------------------------------------------------
    // INITIALISATION
    // --------------------------------------------------

    private void Awake()
    {
        interactUI      = FindObjectOfType<NPCInteractUI>(true);
        dialogueUI      = FindObjectOfType<DialogueUI>(true);
        ultimateChoiceUI = FindObjectOfType<UltimateChoiceUI>(true);
    }

    private void Start()
    {
        if (dialogLines == null || dialogLines.Count == 0)
        {
            Debug.LogWarning($"⚠️ [{name}] Aucun dialogue configuré");
        }
        else
        {
            Debug.Log($"✅ [{name}] Dialogue chargé ({dialogLines.Count} lignes)");
        }
    }

    // --------------------------------------------------
    // UPDATE
    // --------------------------------------------------

    private void Update()
    {
        // Conditions de sécurité
        if (!playerInRange) return;
        if (interactionLocked) return;
        if (dialogueCompleted) return;
        if (dialogueOpen) return;

        bool keyboardPressed =
            Input.GetKeyDown(interactKey);

        bool gamepadPressed =
            Gamepad.current != null &&
            Gamepad.current.buttonNorth.wasPressedThisFrame;

        if (keyboardPressed || gamepadPressed)
        {
            Debug.Log($"🗣️ [{name}] Interaction démarrée");

            OpenDialogue();
        }
    }

    // --------------------------------------------------
    // TRIGGERS
    // --------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        Debug.Log($"🔵 [{name}] Joueur entré");

        // Ne pas afficher si déjà fini
        if (dialogueCompleted) return;

        // Ne pas afficher si interaction verrouillée
        if (interactionLocked) return;

        interactUI ??= FindObjectOfType<NPCInteractUI>(true);

        if (interactUI != null)
        {
            interactUI.Show(interactKey.ToString());
        }
        else
        {
            Debug.LogError("❌ NPCInteractUI introuvable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        Debug.Log($"🔵 [{name}] Joueur sorti");

        interactUI ??= FindObjectOfType<NPCInteractUI>(true);

        if (interactUI != null)
        {
            interactUI.Hide();
        }
    }

    // --------------------------------------------------
    // OUVERTURE DIALOGUE
    // --------------------------------------------------

    private void OpenDialogue()
    {
        // Sécurité anti spam
        if (interactionLocked) return;

        // Mute sons joueur
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.Mute();

        NPCInteractUI interactUI = FindObjectOfType<NPCInteractUI>(true);
        interactionLocked = true;
        dialogueOpen = true;

        // IMPORTANT :
        // verrouille immédiatement le PNJ
        // pour empêcher toute réinterraction
        dialogueCompleted = true;

        Debug.Log($"📖 [{name}] Ouverture dialogue");

        if (interactUI != null)
        {
            interactUI.Hide();
        }

        dialogueUI ??= FindObjectOfType<DialogueUI>(true);

        if (dialogueUI == null)
        {
            Debug.LogError("❌ DialogueUI introuvable");

            ResetInteractionState();
            return;
        }

        dialogueUI.StartDialogue(
            npcPortrait,
            npcName,
            dialogLines,
            OnDialogueFinished
        );
    }

    // --------------------------------------------------
    // FIN DIALOGUE
    // --------------------------------------------------

    private void OnDialogueFinished()
    {
        // Protection ABSOLUE
        if (finishTriggered)
        {
            Debug.LogWarning($"⚠️ [{name}] OnDialogueFinished rappelé plusieurs fois");
            return;
        }

        // Réactive les sons
        if (PlayerSoundManager.Instance != null)
            PlayerSoundManager.Instance.Unmute();

        Debug.Log("✅ Dialogue terminé — ouverture choix ultime");
        finishTriggered = true;

        Debug.Log($"✅ [{name}] Dialogue terminé");

        dialogueOpen = false;

        // --------------------------------------------------
        // CHOIX ULTIME
        // --------------------------------------------------

        if (giveUltimate)
        {
            Debug.Log($"✨ [{name}] Ouverture choix ultime");

            ultimateChoiceUI ??= FindObjectOfType<UltimateChoiceUI>(true);

            if (ultimateChoiceUI != null)
            {
                if (UltimateManager.Instance != null)
                {
                    ultimateChoiceUI.Show(
                        UltimateManager.Instance.GetRandomChoices(3)
                    );
                }
                else
                {
                    Debug.LogError("❌ UltimateManager.Instance NULL");
                }
            }
            else
            {
                Debug.LogError("❌ UltimateChoiceUI introuvable");
            }
        }

        // --------------------------------------------------
        // FIN DE ROOM
        // --------------------------------------------------

        if (StageManager.Instance != null)
        {
            Debug.Log($"🏁 [{name}] OnRoomEnd");

            StageManager.Instance.OnRoomEnd();
        }
        else
        {
            Debug.LogError("❌ StageManager.Instance NULL");
        }
    }

    // --------------------------------------------------
    // RESET EN CAS D'ERREUR
    // --------------------------------------------------

    private void ResetInteractionState()
    {
        interactionLocked = false;
        dialogueOpen = false;
        dialogueCompleted = false;
    }

    // --------------------------------------------------
    // GIZMOS
    // --------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}