// UIStatsPanel.cs
using System.Collections.Generic;
using UnityEngine;

public class UIStatsPanel : MonoBehaviour
{
    public PlayerStats playerStats;
    public StatUIRow rowPrefab;
    public Transform contentRoot;

    public static UIStatsPanel Instance;

    [System.Serializable]
    public class StatIcon
    {
        public StatType statType;
        public Sprite icon;
    }

    public List<StatIcon> statIcons;

    private Dictionary<StatType, StatUIRow> rows = new();

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        TryBindPlayer();
    }

    // Désouscription obligatoire : UIStatsPanel est dans GameScene et se détruit
    // quand on revient au menu. Sans ça, le delegate mort reste sur PlayerStats
    // (DontDestroyOnLoad) et plante ou corrompt la prochaine session.
    void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateAll;
    }

    void Start()
    {
        foreach (var stat in statIcons)
        {
            StatUIRow row = Instantiate(rowPrefab, contentRoot);
            rows[stat.statType] = row;
            row.icon.sprite = stat.icon;
        }

        TryBindPlayer();
        UpdateAll();
    }

    // ─── Binding ─────────────────────────────────────────────────────────────

    void TryBindPlayer()
    {
        // Si pas encore assigné (ou référence perdue), cherche dans la scène
        if (playerStats == null || playerStats.gameObject == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStats = player.GetComponent<PlayerStats>();
        }

        if (playerStats == null) return;

        // Toujours re-souscrire, avec déduplication (- avant + pour éviter les doublons)
        // C'est le cœur du fix : une nouvelle UIStatsPanel doit toujours s'abonner,
        // qu'elle ait trouvé le player ou qu'il soit pré-assigné dans l'Inspector.
        playerStats.OnStatsChanged -= UpdateAll;
        playerStats.OnStatsChanged += UpdateAll;
    }

    bool IsValid()
    {
        return playerStats != null && playerStats.gameObject != null;
    }

    // ─── Preview ─────────────────────────────────────────────────────────────

    public void PreviewBuff(BuffPickupData buff)
    {
        TryBindPlayer();

        if (!IsValid()) return;

        if (buff == null)
        {
            UpdateAll();
            return;
        }

        foreach (var pair in rows)
        {
            StatType statType = pair.Key;

            float current = playerStats.GetStat(statType);
            float preview = playerStats.GetStatPreviewWithBuff(statType, buff.modifiers);

            pair.Value.Set(
                GetSprite(statType),
                current,
                preview,
                statType
            );
        }
    }

    public void ClearPreview()
    {
        UpdateAll();
    }

    // ─── Mise à jour normale ─────────────────────────────────────────────────

    void UpdateAll()
    {
        TryBindPlayer();

        if (!IsValid()) return;

        foreach (var pair in rows)
        {
            StatType stat = pair.Key;
            float value = playerStats.GetStat(stat);

            pair.Value.SetNormal(
                GetSprite(stat),
                value,
                stat
            );
        }
    }

    // ─── Utilitaire ──────────────────────────────────────────────────────────

    Sprite GetSprite(StatType type)
    {
        var entry = statIcons.Find(s => s.statType == type);
        return entry != null ? entry.icon : null;
    }
}