using UnityEngine;

public class StarProcessUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform starIcon;

    private bool isShowed = false;
    private Vector2 originalSize;

    private void Awake()
    {
        originalSize = starIcon.sizeDelta;
        starIcon.sizeDelta = Vector2.zero;
    }

    public void ShowItem()
    {
        if (isShowed) return;

        isShowed = true;

        LeanTween.value(starIcon.gameObject, Vector2.zero, originalSize, 0.5f)
            .setOnUpdate((Vector2 size) => {
                starIcon.sizeDelta = size;
            })
            .setEase(LeanTweenType.easeOutBack); 
    }
}
