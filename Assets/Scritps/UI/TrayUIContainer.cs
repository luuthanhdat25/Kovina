using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrayUIContainer : MonoBehaviour
{
    [SerializeField] private TrayUI trayUI1;
    [SerializeField] private TrayUI trayUI2;
    [SerializeField] private TrayUI trayUI3;

    [SerializeField] private float spacing = 5f; 
    [SerializeField] private float paddingLeft = 5f; 
    [SerializeField] private float paddingRight = 5f;

    private RectTransform containerRect;

    private void Start()
    {
        containerRect = GetComponent<RectTransform>();
        StartCoroutine(AdjustTraysUI());
    }

    private IEnumerator AdjustTraysUI()
    {
        yield return new WaitForSeconds(0.1f);
        AdjustTrayUISizeAndPosition(trayUI1, 0);
        AdjustTrayUISizeAndPosition(trayUI2, 1);
        AdjustTrayUISizeAndPosition(trayUI3, 2);
        OnEnableTraysUI();
    }

    public void OnEnableTraysUI()
    {
        EnableTrayUI(trayUI1);
        EnableTrayUI(trayUI2);
        EnableTrayUI(trayUI3);
    }

    private void AdjustTrayUISizeAndPosition(TrayUI trayUI, int index)
    {
        Image trayImage = trayUI.GetComponent<Image>();
        Sprite traySprite = trayImage.sprite;

        float widthTraySprite = traySprite.texture.width;
        float heightTraySprite = traySprite.texture.height;
        float ratioTraySprite = (float)widthTraySprite / heightTraySprite;

        RectTransform trayRectTransform = trayUI.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        float availableWidth = containerWidth - paddingLeft - paddingRight;
        float newWidth = availableWidth;
        float newHeight = newWidth / ratioTraySprite;

        if (newHeight > (containerHeight - 2 * spacing) / 3)
        {
            newHeight = (containerHeight - 2 * spacing) / 3;
            newWidth = newHeight * ratioTraySprite;
        }

        trayRectTransform.sizeDelta = new Vector2(newWidth, newHeight);

        float totalHeight = 3 * newHeight + 2 * spacing;
        float startY = totalHeight / 2 - newHeight / 2;
        float yOffset = startY - index * (newHeight + spacing);
        trayRectTransform.anchoredPosition = new Vector2((paddingLeft - paddingRight) / 2, yOffset);
    }

    private void EnableTrayUI(TrayUI trayUI)
    {
        trayUI.gameObject.SetActive(true);
        trayUI.EnableScaleAnimation();
    }
}
