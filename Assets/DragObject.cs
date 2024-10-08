using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private bool isDrag;
    private bool canDrag = true;
    private Vector2 startPosition; 

    private void OnMouseDown()
    {
        if (!canDrag) return;
        isDrag = true;
        offset = transform.position - GetTouchPosition();
        startPosition = transform.position;
    }

    private Vector3 GetTouchPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    private void OnMouseUp()
    {
        if (!canDrag) return;
        isDrag = false;
    }

    private void OnMouseDrag()
    {
        if (!canDrag || !isDrag) return;
        transform.position = GetTouchPosition() + offset;
    }
}
