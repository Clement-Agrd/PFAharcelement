using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatTreeManager : MonoBehaviour
{
    public static StatTreeManager Instance { get; private set; }

    public StatTreeData treeData;

    private StatTreeSaveData    saveData  = new StatTreeSaveData();
    private Dictionary<string, int> levels = new Dictionary<string, int>();

    private string SavePath => Application.persistentDataPath + "/stattree.json";

    // ─── Unity ───────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public int GetLevel(StatType stat)
    {
        string key = stat.ToString();
        return levels.ContainsKey(key) ? levels[key] : 0;
    }

    public int GetAvailableXP() => saveData.totalXP - saveData.spentXP;

    public int GetTotalXP() => saveData.totalXP;

    public void AddXP(int amount)
    {
        saveData.totalXP += amount;
        Save();
    }

    public bool CanUpgrade(StatNodeData node)
    {
        int currentLevel = GetLevel(node.statType);
        if (currentLevel >= node.maxLevel) return false;

        int cost = GetCostForNextLevel(node);
        return GetAvailableXP() >= cost;
    }

    public bool Upgrade(StatNodeData node)
    {
        if (!CanUpgrade(node)) return false;

        int cost = GetCostForNextLevel(node);
        saveData.spentXP += cost;

        string key = node.statType.ToString();
        if (!levels.ContainsKey(key)) levels[key] = 0;
        levels[key]++;

        Save();
        return true;
    }

    public int GetCostForNextLevel(StatNodeData node)
    {
        int currentLevel = GetLevel(node.statType);
        return Mathf.RoundToInt(node.baseCost * Mathf.Pow(node.costMultiplier, currentLevel));
    }

    public float GetTotalBonus(StatType stat)
    {
        int level = GetLevel(stat);
        StatNodeData node = GetNode(stat);
        if (node == null) return 0f;
        return node.valuePerLevel * level;
    }

    // Remet tout à zéro et rembourse l'XP
    public void ResetAll()
    {
        saveData.spentXP = 0;
        levels.Clear();
        Save();
        Debug.Log("🔄 Progression réinitialisée");
    }

    // ─── Sauvegarde ──────────────────────────────────────────────────────────

    void Save()
    {
        saveData.nodes.Clear();
        foreach (var kvp in levels)
            saveData.nodes.Add(new NodeSave { statTypeName = kvp.Key, currentLevel = kvp.Value });

        File.WriteAllText(SavePath, JsonUtility.ToJson(saveData, true));
    }

    void Load()
    {
        levels.Clear();

        if (!File.Exists(SavePath)) return;

        saveData = JsonUtility.FromJson<StatTreeSaveData>(File.ReadAllText(SavePath));

        foreach (NodeSave node in saveData.nodes)
            levels[node.statTypeName] = node.currentLevel;
    }

    // ─── Privé ────────────────────────────────────────────────────────────────

    StatNodeData GetNode(StatType stat)
    {
        return treeData.nodes.Find(n => n.statType == stat);
    }
}