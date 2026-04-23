using System;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    private int goldThisRun = 0;
    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        goldThisRun += amount;
        Debug.Log($"💰 Or gagné : +{amount} (total run : {goldThisRun})");
        OnGoldChanged?.Invoke(goldThisRun);
    }

    public int GetGold() => goldThisRun;

    // Appelé en fin de run — convertit l'or en XP permanent

    public void ConvertGoldToXP()
    {
        if (goldThisRun <= 0) return;
        if (StatTreeManager.Instance == null)
        {
            Debug.LogWarning("StatTreeManager manquant !");
            return;
        }

        StatTreeManager.Instance.AddXP(goldThisRun);
        Debug.Log($"✨ {goldThisRun} or converti en XP !");
        goldThisRun = 0;
    }
}