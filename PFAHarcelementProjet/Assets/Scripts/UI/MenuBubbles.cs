// Scripts/UI/Menu/MenuBubbles.cs
using UnityEngine;

public class MenuBubbles : MonoBehaviour
{
    [Header("Bulles")]
    public int   maxBubbles    = 15;
    public float minSize       = 5f;
    public float maxSize       = 20f;
    public float minSpeed      = 50f;
    public float maxSpeed      = 150f;
    public float minSwayAmount = 20f;
    public float maxSwayAmount = 60f;
    public float swaySpeed     = 1.5f;
    public Sprite bubbleSprite;

    [Header("Zone de spawn")]
    public RectTransform spawnArea;

    private BubbleData[] bubbles;
    private float        screenWidth;
    private float        screenHeight;

    struct BubbleData
    {
        public RectTransform rect;
        public float         speed;
        public float         swayAmount;
        public float         swayOffset;
        public float         startX;
    }

    void Start()
    {
        screenWidth  = Screen.width;
        screenHeight = Screen.height;

        bubbles = new BubbleData[maxBubbles];

        for (int i = 0; i < maxBubbles; i++)
            CreateBubble(i, true);
    }

    void CreateBubble(int index, bool randomY)
    {
        GameObject go = new GameObject($"Bubble_{index}");
        go.transform.SetParent(transform, false);

        UnityEngine.UI.Image img = go.AddComponent<UnityEngine.UI.Image>();
        if (bubbleSprite != null)
            img.sprite = bubbleSprite;

        img.color = new Color(1f, 1f, 1f, Random.Range(0.1f, 0.3f));

        RectTransform rect = go.GetComponent<RectTransform>();
        float size = Random.Range(minSize, maxSize);
        rect.sizeDelta = new Vector2(size, size);

        float startX = Random.Range(-screenWidth * 0.5f, screenWidth * 0.5f);
        float startY = randomY
            ? Random.Range(-screenHeight * 0.5f, screenHeight * 0.5f)
            : -screenHeight * 0.5f - size;

        rect.anchoredPosition = new Vector2(startX, startY);

        bubbles[index] = new BubbleData
        {
            rect       = rect,
            speed      = Random.Range(minSpeed, maxSpeed),
            swayAmount = Random.Range(minSwayAmount, maxSwayAmount),
            swayOffset = Random.Range(0f, Mathf.PI * 2f),
            startX     = startX
        };
    }

    void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < bubbles.Length; i++)
        {
            ref BubbleData b = ref bubbles[i];
            if (b.rect == null) continue;

            Vector2 pos = b.rect.anchoredPosition;

            // Monte
            pos.y += b.speed * dt;

            // Balancement horizontal
            pos.x = b.startX + Mathf.Sin(
                Time.time * swaySpeed + b.swayOffset) * b.swayAmount;

            b.rect.anchoredPosition = pos;

            // Reset quand sortie par le haut
            if (pos.y > screenHeight * 0.5f + 30f)
            {
                b.startX = Random.Range(
                    -screenWidth * 0.5f, screenWidth * 0.5f);
                pos.y    = -screenHeight * 0.5f - 30f;
                pos.x    = b.startX;
                b.rect.anchoredPosition = pos;
            }
        }
    }
}