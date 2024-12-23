using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrayUI : DraggableUI
{
    private readonly float DURATION_SCALE = 0.25f;
    private readonly float DURATION_FADE = 0;
    
    [SerializeField] private RectTransform rectTransformParent;
    [SerializeField] private RectTransform pointCenter;
    [SerializeField] private List<Image> imageImages = new List<Image>();

    private int id;
    private Vector3 startPosition;
    private Vector3 pointCenterPosition;
    private bool isPlaySequence = false;
    private bool canDraging = true;

    private void OnEnable()
    {
        id = transform.GetSiblingIndex();
        pointCenterPosition = pointCenter.anchoredPosition;
        startPosition = rectTransform.anchoredPosition;
    }

    public void SetItemImages(List<ItemType> itemTypes)
    {
        var itemSprites = ItemTraditionalManager.Instance.Spawner.GetSpritesForItems(itemTypes);

        // Just show number of item in itemSprites
        imageImages.ForEach(image => image.enabled = false);

        for (int i = 0; i < itemTypes.Count; i++)
        {
            imageImages[i].sprite = itemSprites[i]; 
            imageImages[i].enabled = true;         
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaySequence || !TrayManager.Instance.CanAddTray || !GameManager.Instance.IsGamePlaying())
        {
            canDraging = false;
        }
        else
        {
            canDraging = true;
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isPlaySequence || !TrayManager.Instance.CanAddTray || !canDraging || !GameManager.Instance.IsGamePlaying()) return;
        base.OnDrag(eventData);
       
        if (IsOutsideBounds()) DisableTrayUI(eventData);
        else EnableTrayUI(eventData);
    }

    private void EnableTrayUI(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        SetTrayGameobjectStatus(false);
    }

    private void DisableTrayUI(PointerEventData eventData)
    {
        canvasGroup.alpha = 0f;
        SetTrayGameobjectStatus(true);
    }

    private void SetTrayGameobjectStatus(bool status)
    {
        Tray trayComponent = TrayManager.Instance.GetTraUnplace(id);
        trayComponent.gameObject.SetActive(status);
        trayComponent.SetPositionFollowUserInput();
        trayComponent.SetActiveCell();
    }

    private bool TrackPlaceActiveCell()
    {
        int index = transform.GetSiblingIndex();
        Tray trayComponent = TrayManager.Instance.GetTraUnplace(index);
        trayComponent.ResetCoordinate();

        (bool isPlace, Cell cellPlaced) = trayComponent.SetPlaceInCell();
        SetPlaceStatus(trayComponent, isPlace);

        if (isPlace)
        {
            TrayManager.Instance.OnTrayPlaced(trayComponent, cellPlaced);
        }
        return isPlace;
    }

    private void SetPlaceStatus(Tray trayComponent, bool isPlace)
    {
        Debug.Log($"TrayUI: Set placed status is {isPlace}");
        trayComponent.gameObject.SetActive(isPlace);
        gameObject.SetActive(!isPlace);
    }

    private bool IsOutsideBounds()
    {
        float widthSize = rectTransformParent.rect.width;
        float heightSize = rectTransformParent.rect.height;

        float distanceWidth = Mathf.Abs(rectTransform.anchoredPosition.x - pointCenterPosition.x);
        float distanceHeight = Mathf.Abs(rectTransform.anchoredPosition.y - pointCenterPosition.y);
        
        bool isOutsideWidth = distanceWidth > (widthSize / 2);
        bool isOutsideHeight = distanceHeight > (heightSize / 2);
        return isOutsideWidth || isOutsideHeight; 
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaySequence || !TrayManager.Instance.CanAddTray || !canDraging || !GameManager.Instance.IsGamePlaying()) return;
        base.OnEndDrag(eventData);

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = startPosition;

        EnableScaleAnimation();
        TrackPlaceActiveCell();
    }

    [ContextMenu("Enable scale animation")]
    public void EnableScaleAnimation()
    {
        isPlaySequence = true;
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.scale(rectTransform, Vector3.one * 1.1f, DURATION_SCALE).setEase(LeanTweenType.easeInSine));
        sequence.append(LeanTween.scale(rectTransform, Vector3.one, DURATION_SCALE).setEase(LeanTweenType.easeInSine));
        sequence.append(() => isPlaySequence = false);
    }
}

