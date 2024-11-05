using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private readonly float MOVE_ITEM_DURATION = 0.5F;
    private readonly float COMPLETE_DURATION = 0.5F;
    private readonly float DESPAWN_DURATION = 0.3F;


    public void Init(int id)
    {
        this.id = id;
        positionStart = transform.position;
    }

    public int NumberOfItem() => itemTraditionalList.Count;

    public bool IsContainItem(ItemTraditional itemCheck)
    {
        foreach (var item in itemTraditionalList)
        {
            if (item == itemCheck) return true;
        }
        return false;
    }

    public bool AddItem(ItemTraditional item)
    {
        if (item == null || itemTraditionalList.Count >=3) return false;
        if(IsContainItem(item)) return false;

        itemTraditionalList.Add(item);
        item.transform.SetParent(this.transform);
        return true;
    }

    public float ShortAndMoveItemToPositionOrDespawn()
    {
        var sequence = LeanTween.sequence();
        float timeMatch = 0;

        if (itemTraditionalList.Count > 0)
        {
            itemTraditionalList.Sort((a, b) => a.ItemType.CompareTo(b.ItemType));

            for (int i = 0; i < itemTraditionalList.Count; i++)
            {
                int index = i;
                if (itemTraditionalList[index] == null)
                {
                    itemTraditionalList.Remove(itemTraditionalList[index]);
                }
                else
                {
                    sequence.append(() => MoveItemToPosition(itemTraditionalList[index], points[index]));
                }
            }

            timeMatch += MOVE_ITEM_DURATION + .2f;
            sequence.append(1f); 
            sequence.append(() =>
            {
                if (IsMatch3ItemCompleted())
                {
                    CompletedAndDespawn();
                }
            });
            timeMatch += COMPLETE_DURATION;
        }
        else
        {
            sequence.append(.5f);
            sequence.append(() => Despawn());
            timeMatch += DESPAWN_DURATION;
        }
        return timeMatch;
    }

    public int CountItemsOfType(ItemType itemType)
    {
        return itemTraditionalList.Count(item => item.ItemType == itemType);
    }


    public bool IsMatch3ItemCompleted()
    {
        if(itemTraditionalList.Count < 3) return false;
        ItemType itemTypeBase = itemTraditionalList.First().ItemType;
        for(int i = 1; i< itemTraditionalList.Count; i++)
        {
            if (itemTraditionalList[i].ItemType != itemTypeBase) return false;
        }
        return true;
    }

    private void MoveItemToPosition(ItemTraditional item, Transform pointPosition)
    {
        LeanTween.move(item.gameObject, pointPosition.position, MOVE_ITEM_DURATION).setEase(LeanTweenType.easeInOutQuad);
    }

    public void ClearItemTraditionalList()
    {
        this.itemTraditionalList.Clear();
    }

    public void AddRangeItem(List<ItemTraditional> itemList)
    {
        foreach (var item in itemList)
        {
            if(item != null && itemTraditionalList.Count < 3) 
            {
                itemTraditionalList.Add(item);
                item.transform.SetParent(this.transform);
            }
        }
    }

    public void ChangeItemList(List<ItemTraditional> itemList)
    {
        itemTraditionalList = itemList;
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

    public LTSeq Despawn()
    {
        var sequence = LeanTween.sequence();

        sequence.append(
            LeanTween.scale(gameObject, Vector3.zero, DESPAWN_DURATION)
                .setEase(LeanTweenType.easeInOutQuad)
        );

        sequence.append(() =>
        {
            MainGrid.Instance.ClearTrayInCell(this);
            Destroy(gameObject);
        });

        return sequence;
    }

    public LTSeq CompletedAndDespawn()
    {
        var sequence = LeanTween.sequence();

        sequence.append(
            LeanTween.scale(gameObject, Vector3.zero, COMPLETE_DURATION)
                .setEase(LeanTweenType.easeInOutQuad)
        );

        sequence.append(() =>
        {
            MainGrid.Instance.ClearTrayInCell(this);
            Destroy(gameObject);
        });

        return sequence;
    }

    private void DestroySelf() => Destroy(gameObject);
}
