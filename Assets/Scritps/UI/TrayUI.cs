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

    private Vector3 startPosition;
    private bool isPlaySequence = false;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaySequence) return;
        base.OnBeginDrag(eventData);

        startPosition = pointCenter.anchoredPosition;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isPlaySequence) return;
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
        int index = transform.GetSiblingIndex();
        Tray trayComponent = TrayManager.Instance.GetTray(index);

        trayComponent.gameObject.SetActive(status);
        trayComponent.SetPositionFollowUserInput();
        trayComponent.SetActiveCell();
    }

    private void TrackPlaceActiveCell()
    {
        int index = transform.GetSiblingIndex();
        Tray trayComponent = TrayManager.Instance.GetTray(index);
        bool isPlace = trayComponent.SetPlaceInCell();

        if (isPlace) trayComponent.gameObject.SetActive(true);
        else trayComponent.gameObject.SetActive(false);
    }

    private bool IsOutsideBounds()
    {
        float widthSize = rectTransformParent.rect.width;
        float heightSize = rectTransformParent.rect.height;

        float distanceWidth = Mathf.Abs(rectTransform.anchoredPosition.x - startPosition.x);
        float distanceHeight = Mathf.Abs(rectTransform.anchoredPosition.y - startPosition.y);
        
        bool isOutsideWidth = distanceWidth > (widthSize / 2);
        bool isOutsideHeight = distanceHeight > (heightSize / 2);
        return isOutsideWidth || isOutsideHeight; 
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaySequence) return;
        base.OnEndDrag(eventData);

        LeanTween.alphaCanvas(canvasGroup, 1f, DURATION_FADE).setEase(LeanTweenType.easeOutQuad);
        rectTransform.anchoredPosition = startPosition;

        EnableScaleAnimation();
        TrackPlaceActiveCell();
    }

    [ContextMenu("Enable scale animation")]
    private void EnableScaleAnimation()
    {
        Vector2 startPivot = rectTransform.pivot;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        isPlaySequence = true;

        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.scale(rectTransform, Vector3.one * 1.1f, DURATION_SCALE).setEase(LeanTweenType.easeInSine));
        sequence.append(LeanTween.scale(rectTransform, Vector3.one, DURATION_SCALE).setEase(LeanTweenType.easeInSine));
        sequence.append(() => rectTransform.pivot = startPivot);
        sequence.append(() => isPlaySequence = false);
    }
}

