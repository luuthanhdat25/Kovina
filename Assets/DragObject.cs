using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset; //Offset to Touch Position
    private Vector2 startPosition; //Return to start Position if it can't find cell
    
    private bool canDrag = true;
    private bool isDrag = false;
    
    private List<Cell> cellInteractList = new List<Cell>();
    private Cell activeCell; 

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

    /// <summary>
    /// Find cell to snap
    /// if find, it snap to neast cell
    /// else it move back to start position
    /// </summary>
    private void OnMouseUp()
    {
        if (!canDrag) return;
        isDrag = false;

        if (activeCell != null)
        {
            transform.position = activeCell.transform.position;
            canDrag = false;

            activeCell.UnActiveSelectVisual();
            activeCell.SetContainObjectTrue();
            activeCell = null;
            canDrag = false;
            cellInteractList.Clear();
        }
        else
        {
            transform.position = startPosition;
        }
    }

    /// <summary>
    /// Move object follow touch and finding neast cell
    /// </summary>
    private void OnMouseDrag()
    {
        if (!canDrag || !isDrag) return;
        transform.position = GetTouchPosition() + offset;

        Cell nearestCell = GetNearestCell();
        if (nearestCell != null && nearestCell != activeCell)
        {
            if (activeCell != null)
            {
                activeCell.UnActiveSelectVisual();
            }

            nearestCell.ActiveSelectVisual();
            activeCell = nearestCell;
        }
    }

    private Cell GetNearestCell()
    {
        Cell targetCell = null;
        if (cellInteractList.Count == 0) return targetCell;

        float minDistance = float.MaxValue;

        foreach (var cell in cellInteractList)
        {
            float distance = Vector2.Distance(cell.GetPosition(), (Vector2)transform.position);
            if (distance < minDistance)
            {
                targetCell = cell;
                minDistance = distance;
            }
        }

        return targetCell;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canDrag || !isDrag) return;

        Cell cell = collision.gameObject.GetComponent<Cell>();
        if (cell != null && !cellInteractList.Contains(cell) && !cell.IsContainObject)
        {
            cellInteractList.Add(cell);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!canDrag || !isDrag) return;

        Cell cell = collision.gameObject.GetComponent<Cell>();
        if (cell != null && cellInteractList.Contains(cell))
        {
            cellInteractList.Remove(cell);

            if (cell == activeCell)
            {
                activeCell.UnActiveSelectVisual();
                activeCell = null;

                // Change cell neast
                Cell nearestCell = GetNearestCell();
                if (nearestCell != null)
                {
                    nearestCell.ActiveSelectVisual();
                    activeCell = nearestCell;
                }
            }
        }
    }
}
