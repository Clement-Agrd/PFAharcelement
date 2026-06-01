// Scripts/UI/Menu/MenuSharkFloat.cs
using UnityEngine;
using UnityEngine.UI;

public class MenuSharkFloat : MonoBehaviour
{
    [Header("Oscillation")]
    public float floatAmplitude = 15f;
    public float floatSpeed     = 1.2f;
    public float tiltAmplitude  = 3f;
    public float tiltSpeed      = 0.8f;

    private RectTransform rect;
    private Vector2       startPos;
    private float         elapsed;

    void Awake()
    {
        rect     = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Oscillation verticale
        float offsetY = Mathf.Sin(elapsed * floatSpeed) * floatAmplitude;

        // Légère rotation
        float tilt = Mathf.Sin(elapsed * tiltSpeed) * tiltAmplitude;

        rect.anchoredPosition = startPos + new Vector2(0f, offsetY);
        rect.localRotation    = Quaternion.Euler(0f, 0f, tilt);
    }
}