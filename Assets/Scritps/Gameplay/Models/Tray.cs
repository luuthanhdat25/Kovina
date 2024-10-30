using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour, IObject
{
    private readonly float Z_COORDINATE_MOVE = -1f;

    [SerializeField] private List<Transform> points;
    private int id;

    public List<Transform> Points => points;
    public int Id => id;
 
    private Vector3 positionStart = Vector3.zero;

    private Cell activeCell;

    private List<Cell> cellInteractList = new List<Cell>();

    private List<ItemTraditional> itemTraditionalList = new List<ItemTraditional>();
    
    public void Init(int id)
    {
        this.id = id;
        positionStart = transform.position;
    }

    public void AddItem(ItemTraditional item)
    {
        if (item == null) return;
        itemTraditionalList.Add(item);
        item.transform.SetParent(this.transform);
    }

    public void ShortAndMoveItemToPosition()
    {
        itemTraditionalList.Sort((a, b) => a.ItemType.CompareTo(b.ItemType));
        for (int i = 0; i < itemTraditionalList.Count; i++)
        {
            MoveItemToPosition(itemTraditionalList[i], points[i]);
        }
    }

    private void MoveItemToPosition(ItemTraditional item, Transform pointPoisiton)
    {
        item.transform.position = pointPoisiton.position;
    }

    public void ClearItemTraditionalList()
    {
        this.itemTraditionalList.Clear();
    }

    public void RemoveItem(ItemTraditional item)
    {
        if (item == null) return;
        if (!itemTraditionalList.Contains(item)) return;
        itemTraditionalList.Remove(item);
    }

    public List<ItemTraditional> GetItemTraditionalsList() => itemTraditionalList;

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

    public (bool, Cell) SetPlaceInCell()
    {
        if (activeCell != null)
        {
            transform.position = activeCell.transform.position;
            activeCell.UnActiveSelectVisual();
            activeCell.SetContainObject(this);

            Cell cellPlace = activeCell;
            activeCell = null;

            cellInteractList.Clear();
            return (true, cellPlace);
        }
        return (false, null);
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

    #region Tray Interact to Cell
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cell cell = collision.gameObject.GetComponent<Cell>();
        if (cell != null && !cellInteractList.Contains(cell) && !cell.IsContainObject())
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

    #endregion
    public void DoAction()
    {
        return;
    }

    public void Despawn()
    {
        Invoke("DestroySelf", 0.5f);
    }

    public void CompletedAndDespawn()
    {
        Invoke("DestroySelf", 0.5f);
    }

    private void DestroySelf() => Destroy(gameObject);
}
