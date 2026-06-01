// Scripts/UI/Menu/MenuFishSchool.cs
using UnityEngine;
using UnityEngine.UI;

public class MenuFishSchool : MonoBehaviour
{
    [Header("Poissons")]
    public int    maxFish       = 8;
    public Sprite fishSprite;
    public float  minSize       = 20f;
    public float  maxSize       = 40f;
    public float  minSpeed      = 80f;
    public float  maxSpeed      = 160f;
    public float  minAlpha      = 0.15f;
    public float  maxAlpha      = 0.4f;

    [Header("Spawn")]
    public float spawnInterval  = 3f;

    private float     screenWidth;
    private float     screenHeight;
    private float     nextSpawn;
    private int       fishCount = 0;

    void Start()
    {
        screenWidth  = Screen.width;
        screenHeight = Screen.height;
        nextSpawn    = 0f;

        // Spawn initial
        for (int i = 0; i < maxFish / 2; i++)
            SpawnFish(true);
    }

    void Update()
    {
        if (fishCount < maxFish && Time.time >= nextSpawn)
        {
            SpawnFish(false);
            nextSpawn = Time.time + spawnInterval;
        }
    }

    void SpawnFish(bool randomX)
    {
        GameObject go = new GameObject("Fish");
        go.transform.SetParent(transform, false);

        Image img = go.AddComponent<Image>();
        if (fishSprite != null)
            img.sprite = fishSprite;

        float size = Random.Range(minSize, maxSize);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size * 0.5f);

        bool goRight = Random.value > 0.5f;

        float startX = goRight
            ? -screenWidth * 0.5f - size
            :  screenWidth * 0.5f + size;

        if (randomX)
            startX = Random.Range(-screenWidth * 0.5f, screenWidth * 0.5f);

        float startY = Random.Range(-screenHeight * 0.4f, screenHeight * 0.4f);

        rect.anchoredPosition = new Vector2(startX, startY);

        // Flip selon la direction
        rect.localScale = new Vector3(goRight ? 1f : -1f, 1f, 1f);

        img.color = new Color(1f, 1f, 1f, Random.Range(minAlpha, maxAlpha));

        float speed = Random.Range(minSpeed, maxSpeed) * (goRight ? 1f : -1f);

        MenuFish fish = go.AddComponent<MenuFish>();
        fish.Init(speed, screenWidth, size, this);

        fishCount++;
    }

    public void OnFishDestroyed()
    {
        fishCount--;
    }
}

public class MenuFish : MonoBehaviour
{
    private float          speed;
    private float          screenWidth;
    private float          size;
    private MenuFishSchool school;
    private RectTransform  rect;

    public void Init(float spd, float sw, float s, MenuFishSchool sc)
    {
        speed       = spd;
        screenWidth = sw;
        size        = s;
        school      = sc;
        rect        = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x += speed * Time.deltaTime;
        rect.anchoredPosition = pos;

        // Détruit quand sorti de l'écran
        bool outRight = speed > 0 && pos.x >  screenWidth * 0.5f + size + 10f;
        bool outLeft  = speed < 0 && pos.x < -screenWidth * 0.5f - size - 10f;

        if (outRight || outLeft)
        {
            school?.OnFishDestroyed();
            Destroy(gameObject);
        }
    }
}