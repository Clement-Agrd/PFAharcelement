// Scripts/UI/HealthBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("Barres")]
    public Image   barFill;          // barre principale
    public Image   barGhost;         // barre fantôme (effet LoL)
    public Image   barBackground;

    [Header("Checkpoints")]
    public Transform   checkpointContainer; // parent des traits de checkpoint
    public GameObject  checkpointPrefab;    // prefab du trait vertical
    public int         hpPerCheckpoint = 50; // un trait tous les X HP

    [Header("Texte")]
    public TextMeshProUGUI hpText;

    [Header("Paramètres")]
    public float maxBarWidth      = 400f; // largeur max de la barre
    public float minBarWidth      = 100f; // largeur au départ (1er niveau)
    public float ghostDelay       = 1f;   // délai avant que le fantôme suive
    public float ghostSpeed       = 3f;   // vitesse du fantôme

    private PlayerHealth playerHealth;
    private float        currentFill;
    private float        ghostFill;
    private float        lastDamageTime;
    private float        currentBarWidth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("⚠️ HealthBarUI : PlayerHealth introuvable");
            return;
        }

        // Branche l'événement de changement de HP
        playerHealth.onHealthChanged.AddListener(OnHealthChanged);

        // Initialise avec les HP de base
        float maxHP = playerHealth.GetMaxHP();
        UpdateBarWidth(maxHP);
        UpdateCheckpoints(maxHP);

        currentFill = 1f;
        ghostFill   = 1f;
        UpdateVisuals(playerHealth.GetCurrentHP(), maxHP);
    }

    void Update()
    {
        // Effet fantôme — suit la barre après un délai
        if (Time.time > lastDamageTime + ghostDelay)
        {
            ghostFill = Mathf.Lerp(ghostFill, currentFill, ghostSpeed * Time.deltaTime);
            if (barGhost != null)
                barGhost.fillAmount = ghostFill;
        }
    }

    //  Appelé par PlayerHealth.onHealthChanged 

    void OnHealthChanged(float current, float max)
    {
        UpdateBarWidth(max);
        UpdateCheckpoints(max);
        UpdateVisuals(current, max);

        lastDamageTime = Time.time;
    }

    // Largeur dynamique de la barre

    void UpdateBarWidth(float maxHP)
    {
        // La barre grandit en fonction des HP max
        // Elle commence à minBarWidth et grandit jusqu'à maxBarWidth
        float baseHP    = 100f; // HP de départ
        float ratio     = Mathf.Clamp01((maxHP - baseHP) / (1000f - baseHP));
        currentBarWidth = Mathf.Lerp(minBarWidth, maxBarWidth, ratio);

        // Applique la largeur au background et aux barres
        SetWidth(barBackground, currentBarWidth);
        SetWidth(barFill,       currentBarWidth);
        SetWidth(barGhost,      currentBarWidth);

        if (checkpointContainer != null)
        {
            RectTransform rt = checkpointContainer.GetComponent<RectTransform>();
            if (rt != null) rt.sizeDelta = new Vector2(currentBarWidth, rt.sizeDelta.y);
        }
    }

    void SetWidth(Image image, float width)
    {
        if (image == null) return;
        RectTransform rt = image.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }

    //  Traits de checkpoint 

    void UpdateCheckpoints(float maxHP)
    {
        if (checkpointContainer == null || checkpointPrefab == null) return;

        // Supprime les anciens traits
        foreach (Transform child in checkpointContainer)
            Destroy(child.gameObject);

        // Ne montre les checkpoints que si la barre est à taille max
        if (currentBarWidth < maxBarWidth - 1f) return;

        int checkpointCount = Mathf.FloorToInt(maxHP / hpPerCheckpoint) - 1;

        for (int i = 1; i <= checkpointCount; i++)
        {
            float ratio = (i * hpPerCheckpoint) / maxHP;
            float posX  = (ratio - 0.5f) * currentBarWidth;

            GameObject trait = Instantiate(checkpointPrefab, checkpointContainer);
            RectTransform rt = trait.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(posX, 0f);
        }
    }

    //  Visuels 

    void UpdateVisuals(float current, float max)
    {
        currentFill = max > 0 ? current / max : 0f;

        if (barFill  != null) barFill.fillAmount  = currentFill;
        if (barGhost != null && Time.time < lastDamageTime + 0.1f)
            barGhost.fillAmount = ghostFill;

        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";

        // Couleur de la barre selon le % de vie
        if (barFill != null)
        {
            if      (currentFill > 0.5f) barFill.color = Color.green;
            else if (currentFill > 0.25f) barFill.color = Color.yellow;
            else                          barFill.color = Color.red;
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }
}