// Scripts/UI/MainMenuUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

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

        // Lance la musique du menu
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();
    }

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

    void ShowMenu() => ShowOnly(panelMenu);

    void ShowOnly(GameObject panel)
    {
        panelMenu     .SetActive(panel == panelMenu);
        panelOptions  .SetActive(panel == panelOptions);
        panelCredits  .SetActive(panel == panelCredits);
        panelStatTree .SetActive(panel == panelStatTree);
        panelCinematic.SetActive(panel == panelCinematic);
    }

    void OnCinematicEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnCinematicEnd;
        LaunchGame();
    }

    void LaunchGame()
    {
        ActivePlayer();
    }
    
    void ActivePlayer()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid())
                continue;

            if (!obj.CompareTag("Player"))
                continue;

            obj.SetActive(true);
        }
        Debug.Log("🎮 Run démarrée - Player activé");
        
        var player = GameObject.FindGameObjectWithTag("Player");
        
        // Reset buffs
        PlayerStats stats = player.GetComponent<PlayerStats>();

        if (stats != null)
            stats.ClearAllModifiers();

        // Reset HP
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null)
            health.ResetPlayer();

        // Reset ultimate
        if (UltimateManager.Instance != null)
            UltimateManager.Instance.ResetUltimate(player);
        
        stats.ResetRunStats();

        FindFirstObjectByType<PickupDetector>()?.ForceRefresh();
        UIStatsPanel.Instance?.ClearPreview();
        BuffUI.Instance?.SetPickup(null);
        
        // Fondu musical avant de charger la scène
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameMusic();

        StartCoroutine(LoadGameAfterFade());
    }

    IEnumerator LoadGameAfterFade()
    {
        // Attend la durée du fondu
        yield return new WaitForSecondsRealtime(
            MusicManager.Instance != null
                ? MusicManager.Instance.fadeDuration
                : 1.5f
        );

        SceneManager.LoadScene(gameSceneName);
    }
}