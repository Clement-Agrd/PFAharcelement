// Scripts/UI/UltimateUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateUI : MonoBehaviour
{
    [Header("Références")]
    public Image           iconImage;
    public Image           cooldownFill;
    public Image           cooldownBackground;
    public TextMeshProUGUI cooldownText;
    public Button          ultimateButton;
    public GameObject      root;

    [Header("Couleurs")]
    public Color readyColor    = new Color(1f,   0.85f, 0f,  1f);
    public Color cooldownColor = new Color(0f,   0f,    0f,  0.7f);
    public Color activeColor   = new Color(1f,   1f,    0.3f, 1f);

    [Header("Animation")]
    public float pulseSpeed     = 3f;
    public float pulseAmplitude = 0.08f;

    private UltimateData currentData;
    private float        elapsed;
    private bool         isPulsing = false;

    void Awake()
    {
        // Cache le root dès Awake avant que Start soit appelé
        if (root != null)
            root.SetActive(false);
    }

    void Start()
    {
        if (ultimateButton != null)
            ultimateButton.onClick.AddListener(OnButtonClicked);

        Debug.Log("✅ UltimateUI initialisé");
    }

    void Update()
    {
        if (currentData == null) return;
        UpdateCooldownUI();
        HandlePulse();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public void SetUltimate(UltimateData data)
    {
        currentData = data;

        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (root != null)
        {
            root.SetActive(true);
            Debug.Log($"✅ UltimateUI visible — {data.ultimateName}");
        }
        else
            Debug.LogError("❌ UltimateUI : root non assigné");
    }

    public void OnUnlock()
    {
        if (root != null)
            root.SetActive(true);

        Debug.Log("✅ UltimateUI débloqué");
    }

    public void OnActivate()
    {
        isPulsing = true;
        elapsed   = 0f;
    }

    // ─── Cooldown ─────────────────────────────────────────────────────────────

    void UpdateCooldownUI()
    {
        if (currentData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float remaining = 0f;
        float ratio     = 0f;
        bool  isActive  = false;
        bool  isReady   = false;

        switch (currentData.type)
        {
            case UltimateType.SpecialAttack:
                SpecialAttack sa = player.GetComponent<SpecialAttack>();
                if (sa != null)
                {
                    remaining = sa.GetCooldownRemaining();
                    ratio     = sa.GetCooldownRatio();
                    isReady   = sa.IsReady();
                }
                break;

            case UltimateType.StatBoost:
                StatBoostUltimate sb = player.GetComponent<StatBoostUltimate>();
                if (sb != null)
                {
                    remaining = sb.GetRemaining();
                    ratio     = sb.GetCooldownRatio();
                    isActive  = sb.IsActive;
                    isReady   = sb.IsReady();
                }
                break;

            case UltimateType.Shield:
                ShieldUltimate sh = player.GetComponent<ShieldUltimate>();
                if (sh != null)
                {
                    remaining = sh.GetRemaining();
                    ratio     = sh.GetCooldownRatio();
                    isActive  = sh.IsActive;
                    isReady   = sh.IsReady();
                }
                break;

            case UltimateType.SwarmOfPredators:
                SwarmOfPredatorsUltimate sw = player.GetComponent<SwarmOfPredatorsUltimate>();
                if (sw != null)
                {
                    remaining = sw.GetRemaining();
                    ratio     = sw.GetCooldownRatio();
                    isActive  = sw.IsActive;
                    isReady   = sw.IsReady();
                }
                break;

            case UltimateType.SonicShockwave:
                SonicShockwaveUltimate ss = player.GetComponent<SonicShockwaveUltimate>();
                if (ss != null)
                {
                    remaining = ss.GetRemaining();
                    ratio     = ss.GetCooldownRatio();
                    isActive  = ss.IsActive;
                    isReady   = ss.IsReady();
                }
                break;

            case UltimateType.Mirror:
                MirrorUltimate mi = player.GetComponent<MirrorUltimate>();
                if (mi != null)
                {
                    remaining = mi.GetRemaining();
                    ratio     = mi.GetCooldownRatio();
                    isActive  = mi.IsActive;
                    isReady   = mi.IsReady();
                }
                break;

            case UltimateType.Invisibility:
                InvisibilityUltimate inv = player.GetComponent<InvisibilityUltimate>();
                if (inv != null)
                {
                    remaining = inv.GetRemaining();
                    ratio     = inv.GetCooldownRatio();
                    isActive  = inv.IsActive;
                    isReady   = inv.IsReady();
                }
                break;

            case UltimateType.WeightOfSilence:
                WeightOfSilenceUltimate ws = player.GetComponent<WeightOfSilenceUltimate>();
                if (ws != null)
                {
                    remaining = ws.GetRemaining();
                    ratio     = ws.GetCooldownRatio();
                    isActive  = ws.IsActive;
                    isReady   = ws.IsReady();
                }
                break;

            case UltimateType.Regeneration:
                RegenerationUltimate rg = player.GetComponent<RegenerationUltimate>();
                if (rg != null)
                {
                    remaining = rg.GetRemaining();
                    ratio     = rg.GetCooldownRatio();
                    isActive  = rg.IsActive;
                    isReady   = rg.IsReady();
                }
                break;
        }

        if (cooldownFill != null)
            cooldownFill.fillAmount = isReady ? 0f : ratio;

        if (cooldownBackground != null)
            cooldownBackground.color = isReady
                ? new Color(0f, 0f, 0f, 0f)
                : cooldownColor;

        if (cooldownText != null)
            cooldownText.text = (isActive || isReady) ? "" : $"{remaining:F1}";

        if (iconImage != null)
        {
            if (isActive)     iconImage.color = activeColor;
            else if (isReady) iconImage.color = Color.white;
            else              iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        if (ultimateButton != null)
            ultimateButton.interactable = isReady || isActive;

        if (!isActive) isPulsing = false;
    }

    // ─── Pulse ────────────────────────────────────────────────────────────────

    void HandlePulse()
    {
        if (root == null) return;

        if (!isPulsing)
        {
            root.transform.localScale = Vector3.one;
            return;
        }

        elapsed += Time.deltaTime;
        float scale = 1f + Mathf.Sin(elapsed * pulseSpeed) * pulseAmplitude;
        root.transform.localScale = Vector3.one * scale;
    }

    // ─── Bouton ───────────────────────────────────────────────────────────────

    void OnButtonClicked()
    {
        if (currentData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        switch (currentData.type)
        {
            case UltimateType.SpecialAttack:
                player.GetComponent<SpecialAttack>()?.OnSpecialButtonPressed();
                break;
            case UltimateType.StatBoost:
                player.GetComponent<StatBoostUltimate>()?.Activate();
                break;
            case UltimateType.Shield:
                player.GetComponent<ShieldUltimate>()?.Activate();
                break;
            case UltimateType.SwarmOfPredators:
                player.GetComponent<SwarmOfPredatorsUltimate>()?.Activate();
                break;
            case UltimateType.SonicShockwave:
                player.GetComponent<SonicShockwaveUltimate>()?.Activate();
                break;
            case UltimateType.Mirror:
                player.GetComponent<MirrorUltimate>()?.Activate();
                break;
            case UltimateType.Invisibility:
                player.GetComponent<InvisibilityUltimate>()?.Activate();
                break;
            case UltimateType.WeightOfSilence:
                player.GetComponent<WeightOfSilenceUltimate>()?.Activate();
                break;
            case UltimateType.Regeneration:
                player.GetComponent<RegenerationUltimate>()?.Activate();
                break;
        }
    }
}