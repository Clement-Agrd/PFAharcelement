// Scripts/UI/MainMenuUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panneaux")]
    public GameObject panelMenu;
    public GameObject panelOptions;
    public GameObject panelCredits;
    public GameObject panelStatTree;
    public GameObject panelCinematic;

    [Header("Cinématique")]
    public VideoPlayer videoPlayer;
    public string      gameSceneName = "GameScene";

    void Start()
    {
        ShowMenu();
    }

    // ─── Boutons menu principal ───────────────────────────────────────────────

    public void OnPlay()
    {
        if (videoPlayer != null && videoPlayer.clip != null)
        {
            ShowOnly(panelCinematic);
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnCinematicEnd;
        }
        else
        {
            LaunchGame();
        }
    }

    public void OnOptions()  => ShowOnly(panelOptions);
    public void OnCredits()  => ShowOnly(panelCredits);
    public void OnStatTree() => ShowOnly(panelStatTree);
    public void OnQuit()     => Application.Quit();
    public void OnBack()     => ShowMenu();

    // ─── Privé ────────────────────────────────────────────────────────────────

    void ShowMenu() => ShowOnly(panelMenu);

    void ShowOnly(GameObject panel)
    {
        panelMenu      .SetActive(panel == panelMenu);
        panelOptions   .SetActive(panel == panelOptions);
        panelCredits   .SetActive(panel == panelCredits);
        panelStatTree  .SetActive(panel == panelStatTree);
        panelCinematic .SetActive(panel == panelCinematic);
    }

    void OnCinematicEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnCinematicEnd;
        LaunchGame();
    }

    void LaunchGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}