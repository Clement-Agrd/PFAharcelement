// Scripts/UI/Menu/MenuButtonHover.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale")]
    public float hoverScale  = 1.1f;
    public float normalScale = 1f;
    public float scaleSpeed  = 8f;

    [Header("Couleur")]
    public Color normalColor = Color.white;
    public Color hoverColor  = new Color(0.8f, 0.95f, 1f, 1f);
    public float colorSpeed  = 8f;

    private RectTransform            rect;
    private UnityEngine.UI.Text      text;
    private UnityEngine.UI.Graphic   graphic;
    private float                    targetScale  = 1f;
    private Color                    targetColor;
    private bool                     isHovered    = false;

    void Awake()
    {
        rect        = GetComponent<RectTransform>();
        graphic     = GetComponentInChildren<UnityEngine.UI.Graphic>();
        targetColor = normalColor;

        if (graphic != null)
            graphic.color = normalColor;
    }

    void Update()
    {
        // Scale smooth
        float currentScale = rect.localScale.x;
        float newScale      = Mathf.Lerp(currentScale, targetScale,
                              Time.deltaTime * scaleSpeed);
        rect.localScale = Vector3.one * newScale;

        // Couleur smooth
        if (graphic != null)
            graphic.color = Color.Lerp(graphic.color, targetColor,
                            Time.deltaTime * colorSpeed);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        isHovered   = true;
        targetScale = hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData data)
    {
        isHovered   = false;
        targetScale = normalScale;
        targetColor = normalColor;
    }

    public void OnPointerClick(PointerEventData data)
    {
        // Petit bounce au clic
        StartCoroutine(ClickBounce());
    }

    System.Collections.IEnumerator ClickBounce()
    {
        float elapsed = 0f;
        float duration = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / duration;
            float s  = isHovered
                ? hoverScale + Mathf.Sin(t * Mathf.PI) * 0.05f
                : normalScale + Mathf.Sin(t * Mathf.PI) * 0.05f;

            rect.localScale = Vector3.one * s;
            yield return null;
        }

        targetScale = isHovered ? hoverScale : normalScale;
    }
}