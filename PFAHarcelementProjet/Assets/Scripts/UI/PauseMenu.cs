using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject panelPause;
    public GameObject panelOptions;

    private bool           isPaused = false;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Pause.performed += OnPauseInput;
    }

    void OnDisable()
    {
        controls.Player.Pause.performed -= OnPauseInput;
        controls.Player.Disable();
    }

    void OnPauseInput(InputAction.CallbackContext ctx)
    {
        if (isPaused) Resume();
        else          Pause();
    }

    public void Pause()
    {
        isPaused       = true;
        Time.timeScale = 0f;
        panelPause    .SetActive(true);
        panelOptions  .SetActive(false);
        VirtualCursorController.Instance?.ShowCursor();
    }

    public void Resume()
    {
        isPaused       = false;
        Time.timeScale = 1f;
        panelPause    .SetActive(false);
        panelOptions  .SetActive(false);
        VirtualCursorController.Instance?.HideCursor();
    }

    public void OnOptions()
    {
        panelPause  .SetActive(false);
        panelOptions.SetActive(true);
    }

    public void OnBackFromOptions()
    {
        panelOptions.SetActive(false);
        panelPause  .SetActive(true);
    }

    public void OnReturnToMenu()
    {
        Time.timeScale = 1f;
        VirtualCursorController.Instance?.HideCursor();
        XPManager.Instance.ConvertGoldToXP();
        SceneManager.LoadScene("MainMenu");
    }
}