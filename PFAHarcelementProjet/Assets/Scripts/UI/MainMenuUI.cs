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

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();
    }

    public void OnPlay()
    {
        if (videoPlayer != null && videoPlayer.clip != null)
        {
            // Affiche le panel cinématique
            ShowOnly(panelCinematic);

            // Prépare la vidéo
            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.loopPointReached -= OnCinematicEnd; // évite les doublons
            videoPlayer.loopPointReached += OnCinematicEnd;
            videoPlayer.Play();

            Debug.Log("🎬 Cinématique lancée");
        }
        else
        {
            Debug.Log("⚠️ Pas de vidéo — lancement direct");
            LaunchGame();
        }
    }

    public void OnOptions()  => ShowOnly(panelOptions);
    public void OnCredits()  => ShowOnly(panelCredits);
    public void OnStatTree() => ShowOnly(panelStatTree);
    public void OnQuit()     => Application.Quit();
    public void OnBack()     => ShowMenu();

    // Appelé quand la cinématique se termine
    void OnCinematicEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnCinematicEnd;
        Debug.Log("🎬 Cinématique terminée — lancement du jeu");
        LaunchGame();
    }

    void ShowMenu() => ShowOnly(panelMenu);

    void ShowOnly(GameObject panel)
    {
        if (panelMenu      != null) panelMenu     .SetActive(panel == panelMenu);
        if (panelOptions   != null) panelOptions  .SetActive(panel == panelOptions);
        if (panelCredits   != null) panelCredits  .SetActive(panel == panelCredits);
        if (panelStatTree  != null) panelStatTree .SetActive(panel == panelStatTree);
        if (panelCinematic != null) panelCinematic.SetActive(panel == panelCinematic);
    }

    void LaunchGame()
    {
        ResetPlayer();

        // Fondu musical
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameMusic();

        StartCoroutine(LoadGameAfterFade());
    }

    void ResetPlayer()
    {
        // Réactive le joueur s'il est désactivé
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid())    continue;
            if (!obj.CompareTag("Player")) continue;
            obj.SetActive(true);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("⚠️ Joueur introuvable pour le reset");
            return;
        }

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

        Debug.Log("🎮 Joueur réinitialisé");
    }

    IEnumerator LoadGameAfterFade()
    {
        yield return new WaitForSecondsRealtime(
            MusicManager.Instance != null
                ? MusicManager.Instance.fadeDuration
                : 1.5f
        );

        SceneManager.LoadScene(gameSceneName);
    }
}