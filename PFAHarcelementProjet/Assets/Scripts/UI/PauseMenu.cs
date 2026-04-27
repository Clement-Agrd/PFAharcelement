// Scripts/UI/PauseMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject panelPause;
    public GameObject panelOptions;

    private bool isPaused = false;

    void Update()
    {
        // Echap sur PC / Start sur manette
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (isPaused) Resume();
            else          Pause();
        }
    }

    // ─── API publique ─────────────────────────────────────────────────────────

    public void Pause()
    {
        isPaused          = true;
        Time.timeScale    = 0f; // gèle le jeu
        panelPause.SetActive(true);
        panelOptions.SetActive(false);
    }

    public void Resume()
    {
        isPaused          = false;
        Time.timeScale    = 1f; // reprend le jeu
        panelPause.SetActive(false);
        panelOptions.SetActive(false);
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
        Time.timeScale = 1f; // important — remet le timeScale à 1 avant de changer de scène
        XPManager.Instance.ConvertGoldToXP();
        SceneManager.LoadScene("MainMenu");
    }
}