using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    private readonly float Z_COORDINATE_MOVE = -1f;

    [SerializeField] private List<Transform> points;
    private int id;

    public List<Transform> Points => points;
    public int Id => id;
 
    private Vector3 positionStart = Vector3.zero;

    private Cell activeCell;

    private List<Cell> cellInteractList = new List<Cell>();
    
    public void Init(int id)
    {
        this.id = id;
        positionStart = transform.position;
    }

    public void ResetCoordinate()
    {
        transform.position = positionStart;
    }

    private Vector3 GetTouchPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = positionStart.z + Z_COORDINATE_MOVE;
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
