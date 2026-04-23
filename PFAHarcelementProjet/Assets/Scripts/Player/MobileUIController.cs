using UnityEngine;
using UnityEngine.InputSystem;

public class MobileUIController : MonoBehaviour
{
    [SerializeField] private GameObject mobileControlsCanvas;


    void Start()
    {
        bool showMobileUI =
            Application.isMobilePlatform || Touchscreen.current != null;

        mobileControlsCanvas.SetActive(showMobileUI);
    }
}