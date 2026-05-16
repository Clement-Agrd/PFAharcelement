// Scripts/UI/DialogueUI.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Références")]
    public GameObject      panelDialogue;
    public Image           portrait;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button          nextButton;
    public TextMeshProUGUI nextButtonText;

    private List<string> lines;
    private int          currentLine = 0;
    private Action       onFinished;

    void Awake()
    {
        Debug.Log("✅ DialogueUI Awake");

        if (panelDialogue == null)
            Debug.LogError("❌ DialogueUI : panelDialogue NON assigné dans l'Inspector");
        if (portrait == null)
            Debug.LogError("❌ DialogueUI : portrait NON assigné");
        if (npcNameText == null)
            Debug.LogError("❌ DialogueUI : npcNameText NON assigné");
        if (dialogueText == null)
            Debug.LogError("❌ DialogueUI : dialogueText NON assigné");
        if (nextButton == null)
            Debug.LogError("❌ DialogueUI : nextButton NON assigné");
        if (nextButtonText == null)
            Debug.LogError("❌ DialogueUI : nextButtonText NON assigné");

        if (panelDialogue != null)
            panelDialogue.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNext);
    }

    public void StartDialogue(Sprite npcPortrait, string npcName,
        List<string> dialogLines, Action onFinishedCallback)
    {
        Debug.Log($"📖 StartDialogue — panelDialogue null ? {panelDialogue == null}");

        if (panelDialogue == null)
        {
            Debug.LogError("❌ panelDialogue null");
            Time.timeScale = 1f;
            onFinishedCallback?.Invoke();
            return;
        }

        lines       = dialogLines;
        onFinished  = onFinishedCallback;
        currentLine = 0;

        if (portrait != null && npcPortrait != null)
            portrait.sprite = npcPortrait;

        if (npcNameText != null)
            npcNameText.text = npcName;

        // Force l'activation et vérifie
        panelDialogue.SetActive(true);
        Debug.Log($"📖 panelDialogue.activeSelf = {panelDialogue.activeSelf}");
        Debug.Log($"📖 panelDialogue.activeInHierarchy = {panelDialogue.activeInHierarchy}");

        Time.timeScale = 0f;
        ShowLine(0);
    }

    void ShowLine(int index)
    {
        Debug.Log($"📖 Ligne {index} : {lines[index]}");

        if (dialogueText != null)
            dialogueText.text = lines[index];

        if (nextButtonText != null)
            nextButtonText.text = index >= lines.Count - 1
                ? "Continuer"
                : "Suivant";
    }

    void OnNext()
    {
        currentLine++;
        if (currentLine >= lines.Count)
            EndDialogue();
        else
            ShowLine(currentLine);
    }

    void EndDialogue()
    {
        Debug.Log("📖 Dialogue terminé");

        if (panelDialogue != null)
            panelDialogue.SetActive(false);

        Time.timeScale = 1f;
        onFinished?.Invoke();
    }
}