// Scripts/UI/HealthBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Barres")]
    public Image barFill;
    public Image barGhost;
    public Image barBackground;

    [Header("Checkpoints")]
    public Transform  checkpointContainer;
    public GameObject checkpointPrefab;
    public int        hpPerCheckpoint = 50;

    [Header("Texte")]
    public TextMeshProUGUI hpText;

    [Header("Paramètres")]
    public float maxBarWidth  = 400f;
    public float minBarWidth  = 100f;
    public float ghostDelay   = 1f;
    public float ghostSpeed   = 3f;

    private PlayerHealth playerHealth;
    private float        currentFill;
    private float        ghostFill;
    private float        lastDamageTime;
    private float        currentBarWidth;

    void Awake()
    {
        // Cherche PlayerHealth dès Awake pour être sûr
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("⚠️ HealthBarUI : PlayerHealth introuvable");
            return;
        }

        playerHealth.onHealthChanged.AddListener(OnHealthChanged);
    }

    void Start()
    {
        if (playerHealth == null) return;

        // Force une première mise à jour
        float maxHP = playerHealth.GetMaxHP();
        UpdateBarWidth(maxHP);
        UpdateCheckpoints(maxHP);

        currentFill = 1f;
        ghostFill   = 1f;

        if (barFill  != null) barFill.fillAmount  = 1f;
        if (barGhost != null) barGhost.fillAmount = 1f;

        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(playerHealth.GetCurrentHP())} / {Mathf.CeilToInt(maxHP)}";
    }

    void Update()
    {
        if (Time.time > lastDamageTime + ghostDelay)
        {
            ghostFill = Mathf.Lerp(ghostFill, currentFill, ghostSpeed * Time.deltaTime);
            if (barGhost != null)
                barGhost.fillAmount = ghostFill;
        }
    }

   public void OnHealthChanged(float current, float max)
    {
        UpdateBarWidth(max);
        UpdateCheckpoints(max);
        UpdateVisuals(current, max);
        lastDamageTime = Time.time;
    }

    void UpdateBarWidth(float maxHP)
    {
        float baseHP    = 100f;
        float ratio     = Mathf.Clamp01((maxHP - baseHP) / (1000f - baseHP));
        currentBarWidth = Mathf.Lerp(minBarWidth, maxBarWidth, ratio);

        SetWidth(barBackground, currentBarWidth);
        SetWidth(barFill,       currentBarWidth);
        SetWidth(barGhost,      currentBarWidth);

        if (checkpointContainer != null)
        {
            RectTransform rt = checkpointContainer.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(currentBarWidth, rt.sizeDelta.y);
        }
    }

    void SetWidth(Image image, float width)
    {
        if (image == null) return;
        RectTransform rt = image.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }

    void UpdateCheckpoints(float maxHP)
    {
        if (checkpointContainer == null || checkpointPrefab == null) return;

        foreach (Transform child in checkpointContainer)
            Destroy(child.gameObject);

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

    void UpdateVisuals(float current, float max)
    {
        currentFill = max > 0 ? current / max : 0f;

        if (barFill != null)
        {
            barFill.fillAmount = currentFill;

            if      (currentFill > 0.5f)  barFill.color = new Color(0f,   0.8f, 0f);
            else if (currentFill > 0.25f) barFill.color = new Color(1f,   0.8f, 0f);
            else                          barFill.color = new Color(0.8f, 0f,   0f);
        }

        if (barGhost != null)
            barGhost.fillAmount = Mathf.Max(ghostFill, currentFill);

        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }
}