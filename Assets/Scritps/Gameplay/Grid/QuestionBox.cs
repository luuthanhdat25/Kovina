using System;
using UnityEngine;

public class QuestionBox : MonoBehaviour, IObject
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

    public float DoAction()
    {
        return 0;
    }

    private float DoAnimation()
    {
        Color spriteColor = spriteRenderer.color;
        spriteRenderer.sortingOrder = 5;
        LeanTween.scale(gameObject, scaleOutSizeMultiplier * transform.localScale, scaleOutDuration).setEase(LeanTweenType.easeOutQuad);
        LeanTween.value(gameObject, spriteColor.a, 0, scaleOutSizeMultiplier).setOnUpdate((float alphaValue) =>
        {
            spriteColor.a = alphaValue;
            spriteRenderer.color = spriteColor;
        }).setEase(LeanTweenType.easeInOutQuad);
        return scaleOutDuration;
    }

    public float DoAction(Cell cellPlaced)
    {
        //Tray trayComponent = TrayManager.Instance.GetTraUnplace(index);
        //TrayManager.Instance.OnTrayPlaced(trayComponent, cellPlaced);
        return DoAnimation();
    }
}
