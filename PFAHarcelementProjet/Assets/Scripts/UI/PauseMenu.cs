using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject panelPause;
    public GameObject panelOptions;
    public GameObject panelBestiary;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        panelPause.SetActive(true);
        panelOptions.SetActive(false);
        panelBestiary.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        panelPause.SetActive(false);
        panelOptions.SetActive(false);
        panelBestiary.SetActive(false);
    }

    public void OnOptions()
    {
        panelPause.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void OnBackFromOptions()
    {
        panelOptions.SetActive(false);
        panelPause.SetActive(true);
    }

    public void OnBestiary()
    {
        panelPause.SetActive(false);
        panelBestiary.SetActive(true);

        FindObjectOfType<BestiaryUI>().ShowPage(0);
    }

    public void OnBackFromBestiary()
    {
        panelBestiary.SetActive(false);
        panelPause.SetActive(true);
    }

    public void OnReturnToMenu()
    {
        Time.timeScale = 1f;
        XPManager.Instance.ConvertGoldToXP();
        SceneManager.LoadScene("MainMenu");
    }
}