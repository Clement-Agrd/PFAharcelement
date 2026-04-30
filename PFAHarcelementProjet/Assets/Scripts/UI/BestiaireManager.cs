using System.Collections.Generic;
using UnityEngine;

public class BestiaryManager : MonoBehaviour
{
    public static BestiaryManager Instance;

    private HashSet<int> unlocked = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void UnlockCreature(int id)
    {
        if (!unlocked.Contains(id))
        {
            unlocked.Add(id);
            PlayerPrefs.SetInt("Bestiary_" + id, 1);
        }
    }

    public bool IsUnlocked(int id)
    {
        if (unlocked.Contains(id))
            return true;

        return PlayerPrefs.GetInt("Bestiary_" + id, 0) == 1;
    }
}