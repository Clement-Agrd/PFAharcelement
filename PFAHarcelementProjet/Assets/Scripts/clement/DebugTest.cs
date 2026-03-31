using UnityEngine;


public class DebugTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StageManager.Instance.LoadNext();
        }
    }
}
