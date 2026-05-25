using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private static bool alreadyExists = false;

    void Awake()
    {
        if (alreadyExists)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        alreadyExists = true;
    }
}