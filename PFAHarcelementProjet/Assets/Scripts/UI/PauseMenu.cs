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
        GameObject obj = GameObject.FindWithTag("Player");
        ResetRun(obj);
    }
    void ResetRun(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.ClearAllModifiers();
            stats.ResetRunStats();
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.ResetPlayer();

        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);

        player.SetActive(false);
        player.transform.position = Vector3.zero;
        SceneManager.LoadScene("MainMenu");
    }
}