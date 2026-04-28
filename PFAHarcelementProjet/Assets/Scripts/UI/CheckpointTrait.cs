// Scripts/UI/CheckpointTrait.cs
using UnityEngine;
using UnityEngine.UI;

public class CheckpointTrait : MonoBehaviour
{
    public Image traitImage;

    void Awake()
    {
        if (traitImage == null)
            traitImage = GetComponent<Image>();

        if (traitImage != null)
            traitImage.color = new Color(0f, 0f, 0f, 0.6f);
    }
}