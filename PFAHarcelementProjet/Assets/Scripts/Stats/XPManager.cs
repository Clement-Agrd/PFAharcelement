using System;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }
    
    [SerializeField] int xpThisRun = 0;
    public event Action<int> OnXPChanged;

    [SerializeField] int goldThisRun = 0;
    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        goldThisRun += amount;

        Debug.Log($"💰 Or gagné : +{amount} (total run : {goldThisRun})");

        OnGoldChanged?.Invoke(goldThisRun);
    }

    public void AddXP(int amount)
    {
        xpThisRun += amount;

        Debug.Log($"✨ XP gagné : +{amount} (total run : {xpThisRun})");

        OnXPChanged?.Invoke(xpThisRun);
    }

    public int GetGold() => goldThisRun;


    // ✅ FIN DE RUN
    public void ConvertGoldToXP()
    {
        int totalXP = goldThisRun + xpThisRun;

        if (totalXP <= 0) return;

        if (StatTreeManager.Instance == null)
        {
            Debug.LogWarning("StatTreeManager manquant !");
            return;
        }

        StatTreeManager.Instance.AddXP(totalXP);

        Debug.Log($"✨ Total converti : {totalXP} XP");

        // ✅ RESET
        goldThisRun = 0;
        xpThisRun = 0;

        // ✅ ULTRA IMPORTANT : update UI après reset
        OnGoldChanged?.Invoke(goldThisRun);
        OnXPChanged?.Invoke(xpThisRun);
    }
}