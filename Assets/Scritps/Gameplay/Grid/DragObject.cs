using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private List<Cell> cellInteractList = new List<Cell>();
    private Cell activeCell; 

    private Vector3 GetTouchPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    public bool SetPlaceInCell()
    {
        if (activeCell != null)
        {
            transform.position = activeCell.transform.position;
            activeCell.UnActiveSelectVisual();
            activeCell.SetContainObjectTrue();
            activeCell = null;
            cellInteractList.Clear();
            return true;
        }
        return false;
    }

    public void SetPositionFollowUserInput()
    {
        transform.position = GetTouchPosition();
    }

    public void SetActiveCell()
    {
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
        Cell cell = collision.gameObject.GetComponent<Cell>();
        if (cell != null && !cellInteractList.Contains(cell) && !cell.IsContainObject)
        {
            cellInteractList.Add(cell);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
