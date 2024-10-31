using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrayUIContainer : MonoBehaviour
{
    [SerializeField] private List<TrayUI> trayUIs = new List<TrayUI>();
    public List<TrayUI> TrayUIs => trayUIs;

    [SerializeField] private float spacing = 5f;
    [SerializeField] private float paddingLeft = 5f;
    [SerializeField] private float paddingRight = 5f;

    private RectTransform containerRect;

    private void Start()
    {
        containerRect = GetComponent<RectTransform>();
        StartCoroutine(AdjustAndEnableTraysUI());
    }

    private IEnumerator AdjustAndEnableTraysUI()
    {
        yield return new WaitForSeconds(0.1f);

        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        float availableWidth = containerWidth - paddingLeft - paddingRight;
        float totalSpacingHeight = (trayUIs.Count - 1) * spacing;
        float maxHeightPerTray = (containerHeight - totalSpacingHeight) / trayUIs.Count;

        for (int i = 0; i < trayUIs.Count; i++)
        {
            AdjustTray(trayUIs[i], i, availableWidth, maxHeightPerTray);
            EnableTrayUI(trayUIs[i]);
        }
    }

    private void AdjustTray(TrayUI trayUI, int index, float availableWidth, float maxHeightPerTray)
    {
        Image trayImage = trayUI.GetComponent<Image>();
        Sprite traySprite = trayImage.sprite;

        if (traySprite == null)
        {
            Debug.LogWarning("[TrayUIContainer] Tray sprite is null.");
            return;
        }

        float spriteWidth = traySprite.texture.width;
        float spriteHeight = traySprite.texture.height;
        float spriteAspectRatio = (float)spriteWidth / spriteHeight;

        float newWidth = availableWidth;
        float newHeight = newWidth / spriteAspectRatio;

        if (newHeight > maxHeightPerTray)
        {
            newHeight = maxHeightPerTray;
            newWidth = newHeight * spriteAspectRatio;
        }

        RectTransform trayRect = trayUI.GetComponent<RectTransform>();
        trayRect.sizeDelta = new Vector2(newWidth, newHeight);

        float totalHeight = trayUIs.Count * newHeight + (trayUIs.Count - 1) * spacing;
        float startY = totalHeight / (trayUIs.Count - 1) - newHeight / (trayUIs.Count - 1);
        float yOffset = startY - index * (newHeight + spacing);
        trayRect.anchoredPosition = new Vector2(paddingLeft, yOffset);
    }

    public void OnEnableTraysUI()
    {
        foreach (TrayUI trayUI in trayUIs)
        {
            EnableTrayUI(trayUI);
        }
    }

    private void EnableTrayUI(TrayUI trayUI)
    {
        trayUI.gameObject.SetActive(true);
        trayUI.EnableScaleAnimation();
    }

    public void SetItemImageForTrayUI(List<ItemType> itemTypes, int index)
    {
        if (index >= 0 && index < trayUIs.Count)
        {
            TrayUI trayUI = trayUIs[index];
            trayUI.SetItemImages(itemTypes);
        }
        else
        {
            Debug.LogWarning($"[TrayUIContainer] Invalid index: {index}. TrayUI not found.");
        }
    }
}
