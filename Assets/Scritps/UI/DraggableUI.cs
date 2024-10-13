using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected RectTransform rectTransform;

    public virtual void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        Debug.Log($"DraggableUI: Begin Dragging {gameObject.name} at position: {rectTransform.anchoredPosition}.");
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePositionOnUI;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform,
            eventData.position, eventData.pressEventCamera, out mousePositionOnUI);
        rectTransform.anchoredPosition = mousePositionOnUI - GetPivotOffset();
    }

    protected virtual Vector2 GetPivotOffset()
    {
        return new Vector2(
           rectTransform.rect.width * rectTransform.pivot.x,
           rectTransform.rect.height * rectTransform.pivot.y
        );
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Debug.Log($"DraggableUI: End Dragging {gameObject.name} at position: {rectTransform.anchoredPosition}.");
    }
}
