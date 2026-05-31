// Scripts/UI/MainMenuUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public Image    displayImage;
    public Sprite[] slides;
    public float    displayDuration = 3f;
    public float    fadeDuration    = 1f;

    [Header("Barre de progression")]
    public Image progressBar;       // Image en mode Filled
    public Image progressBackground; // fond de la barre

    [Header("Scène")]
    public string gameSceneName = "GameScene";

    private bool   skipped      = false;
    private int    totalSlides  = 0;
    private float  totalTime    = 0f;
    private float  elapsed      = 0f;

    void Start()
    {
        ShowMenu();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();
    }

    void Update()
    {
        // Skip avec Espace ou Echap
        if (panelCinematic != null && panelCinematic.activeSelf)
        {
            if ((Input.GetKeyDown(KeyCode.Space) ||
                 Input.GetKeyDown(KeyCode.Escape)) && !skipped)
            {
                skipped = true;
                StopAllCoroutines();
                LaunchGame();
            }
        }
    }

    // ─── Boutons menu ─────────────────────────────────────────────────────────

    public void OnPlay()
    {
        if (slides != null && slides.Length > 0)
        {
            skipped    = false;
            elapsed    = 0f;
            totalSlides = slides.Length;

            // Durée totale : (fade in + display + fade out) * nb slides
            totalTime  = (fadeDuration + displayDuration + fadeDuration)
                         * totalSlides;

            ShowOnly(panelCinematic);
            StartCoroutine(PlaySlideshow());
            StartCoroutine(UpdateProgressBar());
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

    // ─── Barre de progression ────────────────────────────────────────────────

    IEnumerator UpdateProgressBar()
    {
        elapsed = 0f;

        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            if (progressBackground != null)
                progressBackground.gameObject.SetActive(true);
        }

        while (elapsed < totalTime && !skipped)
        {
            elapsed += Time.deltaTime;

            if (progressBar != null)
                progressBar.fillAmount = Mathf.Clamp01(elapsed / totalTime);

            yield return null;
        }

        if (progressBar != null)
            progressBar.fillAmount = 1f;
    }

    // ─── Cinématique ──────────────────────────────────────────────────────────

    IEnumerator PlaySlideshow()
    {
        if (displayImage == null)
        {
            Debug.LogError("❌ Display Image non assignée");
            LaunchGame();
            yield break;
        }

        displayImage.color = new Color(1f, 1f, 1f, 0f);

        foreach (Sprite slide in slides)
        {
            if (skipped) yield break;

            displayImage.sprite = slide;

            // Fade in
            yield return StartCoroutine(Fade(0f, 1f));
            if (skipped) yield break;

            // Attente
            yield return new WaitForSeconds(displayDuration);
            if (skipped) yield break;

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f));
        }

        if (!skipped)
            LaunchGame();
    }

    IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            if (skipped) yield break;

            t += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha,
                          Mathf.Clamp01(t / fadeDuration));

            displayImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        displayImage.color = new Color(1f, 1f, 1f, toAlpha);
    }

    // ─── Lancement du jeu ─────────────────────────────────────────────────────

    void LaunchGame()
    {
        ResetPlayer();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameMusic();

        StartCoroutine(LoadGameAfterFade());
    }

    void ResetPlayer()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid())      continue;
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

    // ─── Utilitaire ──────────────────────────────────────────────────────────

    void ShowMenu() => ShowOnly(panelMenu);

    void ShowOnly(GameObject panel)
    {
        if (panelMenu      != null) panelMenu     .SetActive(panel == panelMenu);
        if (panelOptions   != null) panelOptions  .SetActive(panel == panelOptions);
        if (panelCredits   != null) panelCredits  .SetActive(panel == panelCredits);
        if (panelStatTree  != null) panelStatTree .SetActive(panel == panelStatTree);
        if (panelCinematic != null) panelCinematic.SetActive(panel == panelCinematic);
    }
}