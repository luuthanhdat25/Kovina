using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    [SerializeField]
    private float scaleOutDuration = 1f;

    [SerializeField]
    private float scaleOutSizeMultiplier = 1.5f;

    [SerializeField]
    private Tray trayPrefab;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        Instantiate(trayPrefab, transform.position, Quaternion.identity);
        DoAnimation();
    }

    private void DoAnimation()
    {
        Color spriteColor = spriteRenderer.color;
        spriteRenderer.sortingOrder = 5;
        LeanTween.scale(gameObject, scaleOutSizeMultiplier * transform.localScale, scaleOutDuration).setEase(LeanTweenType.easeOutQuad);
        LeanTween.value(gameObject, spriteColor.a, 0, scaleOutSizeMultiplier).setOnUpdate((float alphaValue) =>
        {
            spriteColor.a = alphaValue;
            spriteRenderer.color = spriteColor;
        }).setEase(LeanTweenType.easeInOutQuad);
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
        Open();
    }
}
