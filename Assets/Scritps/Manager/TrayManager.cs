using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrayManager : Singleton<TrayManager>
{
    private readonly Vector3 TRAY_START_POSITION = new Vector3(1000, 0, 0);
    private readonly int TRAY_UI_MAXIMUM = 3;

    [Header("Components"), Space(6)]
    [SerializeField] private Transform trayPrefab;
    [SerializeField] private Transform trayHolder;
    [SerializeField] private TrayUIContainer trayUIContainer;

    public static event Action OnEnoughTrayPlaced;

    private List<Tray> trayUnplaced = new List<Tray>();
    private List<Tray> trayPlaced = new List<Tray>();
    private Tray trayPresent;

    private int countTrayPlaced = 0;

    private void Start() => SpawnUnplacedTraies();

    private void OnEnable() => RegisterListenEvent_EnoughTrayPlaced();
    private void OnDisable() => UnregisterListenEvent_EnoughTrayPlaced();

    private void RegisterListenEvent_EnoughTrayPlaced()
    {
        Debug.Log("[TrayManager] RegisterListenEvent_EnoughTrayPlaced: | SUCCESS |");
        OnEnoughTrayPlaced += MoveToTrayPlaces;
        OnEnoughTrayPlaced += SpawnUnplacedTraies;
    }

    private void UnregisterListenEvent_EnoughTrayPlaced()
    {
        Debug.Log("[TrayManager] UnregisterListenEvent_EnoughTrayPlaced: | SUCCESS |");
        OnEnoughTrayPlaced -= MoveToTrayPlaces;
        OnEnoughTrayPlaced -= SpawnUnplacedTraies;
    }

    public void OnTrayPlaced(Tray trayComponent, Cell cellPlaced)
    {
        countTrayPlaced = countTrayPlaced + 1;
        if (countTrayPlaced >= TRAY_UI_MAXIMUM)
        {
            countTrayPlaced = 0;
            OnEnoughTrayPlaced?.Invoke();
            trayUIContainer.OnEnableTraysUI();
        }

        //Debug.Log($"Placed Tray {trayComponent.name} in {cellPlaced.name}");
        Tray trayCenter;
        if (cellPlaced.GetContainObject() is Tray tray)
        {
            trayCenter = tray;
        }
        else return;

        // Do Algorithm
        List<Cell> cellsHoriList = MainGrid.Instance.GetCellsHorizontal(cellPlaced);
        List<Cell> cellsVertiList = MainGrid.Instance.GetCellsVertical(cellPlaced);
        if (cellsHoriList.Count == 0 && cellsVertiList.Count == 0) return;

        //Interact with Tray
        List<Tray> trayHoriList = GetTrayListFromCellList(cellsHoriList);
        List<Tray> trayVertiList = GetTrayListFromCellList(cellsVertiList);
        Debug.Log($"Tray [hori: {trayHoriList.Count}, verti: {trayVertiList.Count}]");
        if (trayHoriList.Count == 0 && trayVertiList.Count == 0) return;

        // 2 Tray (Hori or Verti)
        if (trayHoriList.Count + trayVertiList.Count == 1)
        {
            trayHoriList.AddRange(trayVertiList);
            Match2Tray(trayCenter, trayHoriList[0], true);
        }
        else if ((trayHoriList.Count == 0 && trayVertiList.Count > 1) || (trayHoriList.Count > 1 && trayVertiList.Count == 0)) //3 tray (Hori or Verti) 
        {
            trayHoriList.AddRange(trayVertiList);
            Match3TrayStraight(trayCenter, trayHoriList);
        }
    }

    private void Match3TrayStraight(Tray trayCenter, List<Tray> trayAroundList)
    {
        List<Tray> trayInteracts = GetTrayListMatch(trayCenter, trayAroundList);
        if (trayInteracts.Count == 0) return;
        
        if (trayInteracts.Count == 1)
        {
            Match2Tray(trayCenter, trayInteracts[0], true);
        }
        else // 2 Tray Interact
        {
            var itemTrayCenterList = trayCenter.GetItemTraditionalsList();
            if (itemTrayCenterList.Count == 1)
            {

            }

            List<ItemTraditional> itemList = new();
            var itemTrayAroundList = new List<ItemTraditional>();
            trayAroundList.ForEach(tray => itemTrayAroundList.AddRange(tray.GetItemTraditionalsList()));

            itemList.AddRange(itemTrayCenterList);
            itemList.AddRange(itemTrayAroundList);
        }


    }

    private void Match2Tray(Tray tray1, Tray tray2, bool isShort)
    {
        if (!IsTrayMatchWithTray(tray1, tray2)) return;

        List<ItemTraditional> itemList = new();
        List<ItemTraditional> tray1ItemList = tray1.GetItemTraditionalsList();
        List<ItemTraditional> tray2ItemList = tray2.GetItemTraditionalsList();
        itemList.AddRange(tray1ItemList);
        itemList.AddRange(tray2ItemList);

        if (itemList.Count == 2)
        {
            var itemMove = tray1ItemList[0];
            tray1.RemoveItem(itemMove);
            tray2.AddItem(itemMove);
        }
        else
        {
            itemList.Sort((item1, item2) => item1.ItemType.CompareTo(item2.ItemType));

            (ItemType itemTypeMostFrequence, int countFrequence) = GetMostFrequentItemType(itemList);
            Debug.Log($"Most frequence Item: {itemTypeMostFrequence.ToString()}, {countFrequence}");

            if (countFrequence >= 3)
            {
                int count = 0;
                foreach (var item in tray2ItemList)
                {
                    if (item.ItemType == itemTypeMostFrequence) count++;
                }
                Tray trayHave2Item = count == 2 ? tray2 : tray1;
                Tray trayHave1Item = trayHave2Item == tray2 ? tray1 : tray2;
                ItemTraditional itemMoveToMatch = trayHave1Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType == itemTypeMostFrequence);
                ItemTraditional itemMoveToOther = trayHave2Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != itemTypeMostFrequence);

                trayHave2Item.RemoveItem(itemMoveToOther);
                trayHave1Item.RemoveItem(itemMoveToMatch);
                trayHave1Item.AddItem(itemMoveToOther);
                trayHave2Item.AddItem(itemMoveToMatch);
            }
            else //2 or move frequence
            {
                if (tray1ItemList.Count == 1 || tray1ItemList.Count == 2)
                {
                    Tray trayHave1Item = tray1ItemList.Count == 1 ? tray1 : tray2;
                    Tray trayOther = trayHave1Item == tray2 ? tray1 : tray2;
                    ItemTraditional itemMoveToMatch = trayOther.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType == itemTypeMostFrequence);

                    trayOther.RemoveItem(itemMoveToMatch);
                    trayHave1Item.AddItem(itemMoveToMatch);
                }
                else
                {
                    List<ItemTraditional> itemListFrequence = new List<ItemTraditional>();
                    List<ItemTraditional> itemListOther = new List<ItemTraditional>();
                    foreach (var item in itemList)
                    {
                        if (item.ItemType == itemTypeMostFrequence)
                        {
                            itemListFrequence.Add(item);
                        }
                        else
                        {
                            itemListOther.Add(item);
                        }
                    }

                    if (itemListOther.Count == 4)
                    {
                        itemListOther.Sort((item1, item2) => item1.ItemType.CompareTo(item2.ItemType));
                        (ItemType itemTypeSecondFrequence, int countFrequence2) = GetMostFrequentItemType(itemListOther);

                        ItemTraditional itemOther2;
                        if (countFrequence2 == 1) itemOther2 = itemListOther.First();
                        else itemOther2 = itemListOther.FirstOrDefault(item => item.ItemType != itemTypeSecondFrequence);

                        if (itemOther2 != null)
                        {
                            itemListFrequence.Add(itemOther2);
                            itemListOther.Remove(itemOther2);
                        }
                    }

                    tray1.ClearItemTraditionalList();
                    tray2.ClearItemTraditionalList();
                    tray2.AddRangeItem(itemListFrequence);

                    itemListOther.Sort((itemA, itemB) => itemA.ItemType.CompareTo(itemB.ItemType));
                    tray1.AddRangeItem(itemListOther);
                }
            }
        }
        
        if (isShort)
        {
            tray1.ShortAndMoveItemToPositionOrDespawn();
            tray2.ShortAndMoveItemToPositionOrDespawn();
        }
    }

    private (ItemType, int count) GetMostFrequentItemType(List<ItemTraditional> sortedItemList)
    {
        ItemType mostFrequent = sortedItemList[0].ItemType;
        int maxCount = 1, currentCount = 1;

        for (int i = 1; i < sortedItemList.Count; i++)
        {
            currentCount = sortedItemList[i].ItemType == sortedItemList[i - 1].ItemType ? currentCount + 1 : 1;

            if (currentCount > maxCount)
            {
                maxCount = currentCount;
                mostFrequent = sortedItemList[i].ItemType;
            }
        }
        return (mostFrequent, maxCount);
    }


    private List<Tray> GetTrayListFromCellList(List<Cell> cellList)
    {
        List<Tray> trayList = new List<Tray>();
        if (cellList == null || cellList.Count == 0) return trayList;
        foreach (var cell in cellList)
        {
            if (cell.GetContainObject() is Tray tray)
            {
                trayList.Add(tray);
            }
        }
        return trayList;
    }

    private bool IsTrayMatchWithTray(Tray tray1, Tray tray2)
    {
        var requireItems = tray2.GetItemTraditionalsList();
        var checkItems = tray1.GetItemTraditionalsList();
        foreach (var item in checkItems)
        {
            var existItem = requireItems.FirstOrDefault(reItem => reItem.ItemType == item.ItemType);
            if (existItem != null) return true;
        }
        return false;
    }

    private List<Tray> GetTrayListMatch(Tray trayCheck, List<Tray> trayList)
    {
        List<Tray> trayListMath = new List<Tray>();
        if (trayList.Count == 0) return trayListMath;

        List<ItemType> itemCheckTypeList = new List<ItemType>();

        foreach (var item in trayCheck.GetItemTraditionalsList())
        {
            if (!itemCheckTypeList.Contains(item.ItemType))
            {
                itemCheckTypeList.Add(item.ItemType);
            }
        }

        foreach (var itemType in itemCheckTypeList)
        {
            foreach (var tray in trayList)
            {
                var existItem = tray.GetItemTraditionalsList()
                                    .FirstOrDefault(reItem => reItem.ItemType == itemType);
                if (existItem != null && !trayListMath.Contains(tray))
                {
                    trayListMath.Add(tray);
                }
            }
        }
        return trayListMath;
    }


    private void SpawnUnplacedTraies()
    {
        trayUnplaced.Clear();
        for (int i = 0; i < TRAY_UI_MAXIMUM; i++)
        {
            Debug.Log($"[TrayManager] OnTrayPlaced: {i}");
            Tray trayComponent = CreateUnplacedTray(i, TRAY_START_POSITION);
            trayComponent.gameObject.SetActive(false);
            CreateItemOnTray(trayComponent, i);
        }
    }

    private void MoveToTrayPlaces()
    {
        for (int i = 0; i < trayUnplaced.Count; i++)
            trayPlaced.Add(trayUnplaced[i]);
    }

    private void CreateItemOnTray(Tray trayComponent, int index)
    {
        var points = trayComponent.Points;
        var itemTypes = new List<ItemType>();
        trayComponent.ClearItemTraditionalList();

        int numberOffItemOnTray = UnityEngine.Random.Range(1, 4); // Random number of items on Tray

        for (int i = 0; numberOffItemOnTray > 0; i++, numberOffItemOnTray--)
        {
            ItemType itemType;

            do
            {
                itemType = ItemTraditionalManager.Instance.Spawner.GetRandomEnumValue();
            } while (itemTypes.Count >= 2 && itemTypes[itemTypes.Count - 1] == itemType && itemTypes[itemTypes.Count - 2] == itemType);

            var item = ItemTraditionalManager.Instance.Spawner.Spawn(itemType);
            trayComponent.AddItem(item);

            item.transform.SetParent(trayComponent.transform);
            item.transform.position = points[i].position;
            item.transform.localScale = points[i].localScale;

            itemTypes.Add(itemType);
        }

        itemTypes.Sort((itemA, itemB) => itemA.CompareTo(itemB));
        trayUIContainer.SetItemImageForTrayUI(itemTypes, index);
    }


    #region CRUD

    //CREATE -------------------------------------------------------------
    public Tray CreateUnplacedTray(int index, Vector3 trayPosition = default(Vector3), Quaternion quaternion = default(Quaternion))
    {
        //TODO: Upgrade to object pooling design pattern to optimize performance
        Transform newTray = Instantiate(trayPrefab, trayPosition, quaternion);
        if (newTray == null || newTray.GetComponent<Tray>() == null)
        {
            Debug.LogWarning("Developer, please check the prefab configuration & ensure it has a Tray component attached.");
            Debug.LogWarning("TrayManager: trayPrefab don't have Tray component to add |FAIL|");
            return null;
        }

        Tray trayComponent = newTray.GetComponent<Tray>();
        AddNewTrayToUnplaced(trayComponent);
        trayComponent.Init(index);

        newTray.SetParent(trayHolder);
        return trayComponent;
    }

    private void AddNewTrayToUnplaced(Tray trayComponent)
    {
        if (trayUnplaced == null)
        {
            Debug.Log("TrayManager: trayUnplaced has not been initialized. Initializing now...");
            trayUnplaced = new List<Tray>();
        }

        trayUnplaced.Add(trayComponent);
        Debug.Log($"TrayManager: add {trayComponent} element to trayUnplaced |SUCCESS|");
    }

    private void AddNewTrayToPlaced(int index)
    {
        if (trayPlaced == null)
        {
            Debug.Log("TrayManager: trayPlaced has not been initialized. Initializing now...");
            trayPlaced = new List<Tray>();
        }

        if (trayUnplaced[index] == null)
        {
            Debug.LogWarning($"TrayManager: Tray at index {index} is null, cannot add to placed |FAIL|");
            return;
        }

        Tray trayComponent = trayUnplaced[index];
        trayPlaced.Add(trayComponent);
        Debug.Log($"TrayManager: add {trayComponent} element to trayPlaced |SUCCESS|");
    }

    //READ ----------------------------------------------------------------
    public Tray GetTraUnplace(int index)
    {
        if (index < 0 || index >= trayUnplaced.Count)
        {
            Debug.LogWarning($"Developer, Please ensure the index is within the range (0 to {trayUnplaced.Count - 1})");
            Debug.LogWarning($"TrayManager: Invalid index {index}.");
            return null;
        }
        return trayUnplaced[index];
    }

    //UPDATE --------------------------------------------------------------
    public void EnableTray(int index) => ChangeTrayStatusEnable(index, true);
    public void DisableTray(int index) => ChangeTrayStatusEnable(index, false);

    private void ChangeTrayStatusEnable(int index, bool status)
    {
        Tray trayComponent = GetTraUnplace(index);
        if (trayComponent == null)
        {
            Debug.LogWarning($"TrayManager: Tray at index {index} is null or doesn't exist |FAIL|");
            return;
        }
        trayComponent.gameObject.SetActive(status);
    }

    #endregion
}