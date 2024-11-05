using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    private bool canAddTray = true;
    public bool CanAddTray => canAddTray;

    //Dat Add
    public class CompletedMatchItemEventArg : EventArgs
    {
        public ItemType ItemType;
    }

    public EventHandler<CompletedMatchItemEventArg> OnCompletedMatchItem;

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

    #region Tray Match Algorithm
    public void OnTrayPlaced(Tray trayPlaced, Cell cellPlaced)
    {
        countTrayPlaced = countTrayPlaced + 1;
        if (countTrayPlaced >= TRAY_UI_MAXIMUM)
        {
            countTrayPlaced = 0;
            OnEnoughTrayPlaced?.Invoke();
            trayUIContainer.OnEnableTraysUI();
        }
        trayPlaced.SetCellPlaced(cellPlaced);

        // Do Algorithm
        List<Cell> cellsHoriList = MainGrid.Instance.GetCellsHorizontal(cellPlaced);
        List<Cell> cellsVertiList = MainGrid.Instance.GetCellsVertical(cellPlaced);
        if (cellsHoriList.Count == 0 && cellsVertiList.Count == 0) return;

        //Interact with Tray
        List<Tray> trayHoriList = GetTrayListFromCellList(cellsHoriList);
        List<Tray> trayVertiList = GetTrayListFromCellList(cellsVertiList);
        Debug.Log($"Tray [hori: {trayHoriList.Count}, verti: {trayVertiList.Count}]");
        if (trayHoriList.Count == 0 && trayVertiList.Count == 0) return;

        float timeMatch = 0;
        
        if (trayHoriList.Count + trayVertiList.Count == 1) // 2 Tray (Hori or Verti)
        {
            trayHoriList.AddRange(trayVertiList);
            timeMatch += Match2Tray(trayPlaced, trayHoriList[0], true);
        }
        else if ((trayHoriList.Count == 0 && trayVertiList.Count > 1) || (trayHoriList.Count > 1 && trayVertiList.Count == 0)) //3 tray (Hori or Verti) 
        {
            trayHoriList.AddRange(trayVertiList);
            timeMatch += Match3Tray(trayPlaced, trayHoriList);
        }
        else // >3 Tray (Hori and Verti)
        {
            timeMatch += MatchTrayPlus(trayPlaced, trayHoriList, trayVertiList);
        }
        Debug.Log("[TrayManager]Delay time: " + timeMatch);
        StartCoroutine(WaitForMatchCompleted(timeMatch));
    }

    private IEnumerator WaitForMatchCompleted(float timeMatch)
    {
        canAddTray = false;
        yield return new WaitForSeconds(timeMatch);
        canAddTray = true;
    }

    private float MatchTrayPlus(Tray trayCenter, List<Tray> trayHoriList, List<Tray> trayVertiList)
    {
        var trayAroundList = new List<Tray>();
        trayAroundList.AddRange(trayHoriList);
        trayAroundList.AddRange(trayVertiList);
        List<Tray> trayInteracts = GetTrayListMatch(trayCenter, trayAroundList);
        if (trayInteracts.Count == 0) return 0f;

        float timeMatch = 0;
        if (trayInteracts.Count == 1)
        {
            timeMatch += Match2Tray(trayCenter, trayInteracts[0], true);
        }
        else if(trayInteracts.Count == 2)
        {
            timeMatch += Match3Tray(trayCenter, trayInteracts);
        }
        else // >= 3 Tray
        {
            var itemTrayCenterList = trayCenter.GetItemTraditionalsList();
            itemTrayCenterList.Sort((item1, item2) => item1.ItemType.CompareTo(item2.ItemType));
            (ItemType itemTypeMostFrequence, int countFrequence) = GetMostFrequentItemType(itemTrayCenterList);

            if (countFrequence == 2 && GetFirstTrayHasNumberOfItem(itemTypeMostFrequence, trayAroundList, 2) == null)
            {
                Tray firstTrayHas1Item = GetFirstTrayHasNumberOfItem(itemTypeMostFrequence, trayAroundList, 1);
                ItemTraditional itemMatch = firstTrayHas1Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType == itemTypeMostFrequence);
                ItemTraditional itemNotMatch = trayCenter.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != itemTypeMostFrequence);
                firstTrayHas1Item.RemoveItem(itemMatch);
                trayCenter.RemoveItem(itemNotMatch);

                trayCenter.AddItem(itemMatch);
                firstTrayHas1Item.AddItem(itemNotMatch);

                var duration1 = trayCenter.ShortAndMoveItemToPositionOrDespawn();
                var duration2 = firstTrayHas1Item.ShortAndMoveItemToPositionOrDespawn();
                timeMatch += Mathf.Max(duration1, duration2);
            }
            else
            {
                List<ItemTraditional> removeCenterItems = new List<ItemTraditional>();
                List<ItemTraditional> addCenterItem = new List<ItemTraditional>();
                List<Tray> trayListInteracts = new List<Tray>();
                foreach (var item in itemTrayCenterList)
                {
                    ItemType centerItemType = item.ItemType;
                    //List<Tray> trayListHas2Item = GetTrayListHasNumberOfItem(centerItemType, trayAroundList, 2);
                    Tray trayFirstHas2Item = GetFirstTrayHasNumberOfItem(centerItemType, trayAroundList, 2);
                    if (trayFirstHas2Item != null)
                    {
                        if (trayFirstHas2Item.NumberOfItem() == 2)
                        {
                            removeCenterItems.Add(item);
                            trayFirstHas2Item.AddItem(item);
                        }
                        else
                        {
                            ItemTraditional otherItem = trayFirstHas2Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != centerItemType);
                            removeCenterItems.Add(item);
                            addCenterItem.Add(otherItem);
                            trayFirstHas2Item.RemoveItem(otherItem);
                            trayFirstHas2Item.AddItem(item);
                        }
                        trayListInteracts.Add(trayFirstHas2Item);
                    }
                }
                foreach (var item in removeCenterItems)
                {
                    trayCenter.RemoveItem(item);
                }
                //trayCenter.AddRangeItem(addCenterItem);

                if (trayCenter.NumberOfItem() != 0)
                {
                    removeCenterItems.Clear();
                    //addCenterItem.Clear();
                    foreach (var item in trayCenter.GetItemTraditionalsList())
                    {
                        Tray trayFirstHas1Item = GetFirstTrayHasNumberOfItem(item.ItemType, trayAroundList, 1);
                        if (trayFirstHas1Item != null && trayFirstHas1Item.NumberOfItem() < 3)
                        {
                            if (trayFirstHas1Item.NumberOfItem() == 3)
                            {
                                ItemTraditional otherItem = trayFirstHas1Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != item.ItemType);
                                removeCenterItems.Add(item);
                                addCenterItem.Add(otherItem);
                                trayFirstHas1Item.RemoveItem(otherItem);
                                trayFirstHas1Item.AddItem(item);
                            }
                            else
                            {
                                removeCenterItems.Add(item);
                                trayFirstHas1Item.AddItem(item);
                            }
                            if (!trayListInteracts.Contains(trayFirstHas1Item)) trayListInteracts.Add(trayFirstHas1Item);
                        }
                    }

                    foreach (var item in removeCenterItems)
                    {
                        trayCenter.RemoveItem(item);
                    }
                }
                trayCenter.AddRangeItem(addCenterItem);

                float maxTimeShort = float.MinValue;
                float trayCenterDuration = trayCenter.ShortAndMoveItemToPositionOrDespawn();
                maxTimeShort = Mathf.Max(maxTimeShort, trayCenterDuration);

                foreach (var tray in trayListInteracts)
                {
                    float duration = tray.ShortAndMoveItemToPositionOrDespawn();
                    maxTimeShort = Mathf.Max(maxTimeShort, duration);
                }
                timeMatch += maxTimeShort;
            }
        }
        return timeMatch;
    }

    private float Match3Tray(Tray trayCenter, List<Tray> trayAroundList)
    {
        List<Tray> trayInteracts = GetTrayListMatch(trayCenter, trayAroundList);
        if (trayInteracts.Count == 0) return 0f;

        float timeMatch = 0;

        if (trayInteracts.Count == 1)
        {
            timeMatch += Match2Tray(trayCenter, trayInteracts[0], true);
        }
        else // 2 Tray Interact
        {
            var itemTrayCenterList = trayCenter.GetItemTraditionalsList();
            if (itemTrayCenterList.Count == 1) // Tray center has 1 item
            {
                ItemType centerItemType = itemTrayCenterList[0].ItemType;
                Tray firstTrayHas2Item = GetFirstTrayHasNumberOfItem(centerItemType, trayAroundList, 2);

                if(firstTrayHas2Item != null)
                {
                    Match2Tray(trayCenter, firstTrayHas2Item, true);
                }
                else // has 1 item in each tray around
                {
                    foreach (var tray in trayAroundList)
                    {
                        ItemTraditional matchItem = tray.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType == centerItemType);
                        if(matchItem != null)
                        {
                            tray.RemoveItem(matchItem);
                            trayCenter.AddItem(matchItem);
                        }
                    }
                    
                    float maxTimeShort = float.MinValue;
                    float trayCenterDuration = trayCenter.ShortAndMoveItemToPositionOrDespawn();
                    maxTimeShort = Mathf.Max(maxTimeShort, trayCenterDuration);

                    foreach (var tray in trayAroundList)
                    {
                        float duration = tray.ShortAndMoveItemToPositionOrDespawn();
                        maxTimeShort = Mathf.Max(maxTimeShort, duration);
                    }
                    timeMatch += maxTimeShort;
                }
            }
            else
            {
                itemTrayCenterList.Sort((item1, item2) => item1.ItemType.CompareTo(item2.ItemType));
                (ItemType itemTypeMostFrequence, int countFrequence) = GetMostFrequentItemType(itemTrayCenterList);

                if(countFrequence == 2 && GetFirstTrayHasNumberOfItem(itemTypeMostFrequence, trayAroundList, 2) == null)
                {
                    Tray firstTrayHas1Item = GetFirstTrayHasNumberOfItem(itemTypeMostFrequence, trayAroundList, 1);
                    ItemTraditional itemMatch = firstTrayHas1Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType == itemTypeMostFrequence);
                    ItemTraditional itemNotMatch = trayCenter.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != itemTypeMostFrequence);
                    firstTrayHas1Item.RemoveItem(itemMatch);
                    trayCenter.RemoveItem(itemNotMatch);

                    trayCenter.AddItem(itemMatch);
                    firstTrayHas1Item.AddItem(itemNotMatch);

                    var duration1 = trayCenter.ShortAndMoveItemToPositionOrDespawn();
                    var duration2 = firstTrayHas1Item.ShortAndMoveItemToPositionOrDespawn();
                    timeMatch += Mathf.Max(duration1, duration2);
                }
                else
                {
                    List<ItemTraditional> removeCenterItems = new List<ItemTraditional>();
                    List<ItemTraditional> addCenterItem = new List<ItemTraditional>();
                    List<Tray> trayListInteracts = new List<Tray>();
                    foreach (var item in itemTrayCenterList)
                    {
                        ItemType centerItemType = item.ItemType;
                        //List<Tray> trayListHas2Item = GetTrayListHasNumberOfItem(centerItemType, trayAroundList, 2);
                        Tray trayFirstHas2Item = GetFirstTrayHasNumberOfItem(centerItemType, trayAroundList, 2);
                        if (trayFirstHas2Item != null)
                        {
                            if (trayFirstHas2Item.NumberOfItem() == 2)
                            {
                                removeCenterItems.Add(item);
                                trayFirstHas2Item.AddItem(item);
                            }
                            else
                            {
                                ItemTraditional otherItem = trayFirstHas2Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != centerItemType);
                                removeCenterItems.Add(item);
                                addCenterItem.Add(otherItem);
                                trayFirstHas2Item.RemoveItem(otherItem);
                                trayFirstHas2Item.AddItem(item);
                            }
                            trayListInteracts.Add(trayFirstHas2Item);
                        }
                    }
                    foreach (var item in removeCenterItems)
                    {
                        trayCenter.RemoveItem(item);
                    }
                    //trayCenter.AddRangeItem(addCenterItem);

                    if(trayCenter.NumberOfItem() != 0)
                    {
                        removeCenterItems.Clear();
                        //addCenterItem.Clear();
                        foreach (var item in trayCenter.GetItemTraditionalsList())
                        {
                            Tray trayFirstHas1Item = GetFirstTrayHasNumberOfItem(item.ItemType, trayAroundList, 1);
                            if (trayFirstHas1Item != null && trayFirstHas1Item.NumberOfItem() < 3)
                            {
                                if(trayFirstHas1Item.NumberOfItem() == 3)
                                {
                                    ItemTraditional otherItem = trayFirstHas1Item.GetItemTraditionalsList().FirstOrDefault(item => item.ItemType != item.ItemType);
                                    removeCenterItems.Add(item);
                                    addCenterItem.Add(otherItem);
                                    trayFirstHas1Item.RemoveItem(otherItem);
                                    trayFirstHas1Item.AddItem(item);
                                }
                                else
                                {
                                    removeCenterItems.Add(item);
                                    trayFirstHas1Item.AddItem(item);
                                }
                                if(!trayListInteracts.Contains(trayFirstHas1Item))  trayListInteracts.Add(trayFirstHas1Item);
                            }
                        }

                        foreach (var item in removeCenterItems)
                        {
                            trayCenter.RemoveItem(item);
                        }
                    }
                    trayCenter.AddRangeItem(addCenterItem);

                    float maxTimeShort = float.MinValue;
                    float trayCenterDuration = trayCenter.ShortAndMoveItemToPositionOrDespawn();
                    maxTimeShort = Mathf.Max(maxTimeShort, trayCenterDuration);

                    foreach (var tray in trayListInteracts)
                    {
                        float duration = tray.ShortAndMoveItemToPositionOrDespawn();
                        maxTimeShort = Mathf.Max(maxTimeShort, duration);
                    }
                    timeMatch += maxTimeShort;
                }
            }
        }
        return timeMatch;
    }

    private Tray GetFirstTrayHasNumberOfItem(ItemType itemType, List<Tray> trayList, int numberOfItem)
    {
        if (trayList == null || trayList.Count == 0 || numberOfItem > 2 || numberOfItem < 1) return null;

        foreach (Tray tray in trayList)
        {
            int numberOfItemInTray = tray.CountItemsOfType(itemType);
            if (numberOfItemInTray == numberOfItem) return tray;
        }
        return null;
    }

    private List<Tray> GetTrayListHasNumberOfItem(ItemType itemType, List<Tray> trayList, int numberOfItem)
    {
        List<Tray> returnList = new List<Tray>();
        if (trayList == null || trayList.Count == 0 || numberOfItem > 2 || numberOfItem < 1) return returnList;

        foreach (Tray tray in trayList)
        {
            int numberOfItemInTray = tray.CountItemsOfType(itemType);
            if (numberOfItemInTray == numberOfItem) returnList.Add(tray);
        }
        return returnList;
    }

    private float Match2Tray(Tray tray1, Tray tray2, bool isShort)
    {
        if (!IsTrayMatchWithTray(tray1, tray2)) return 0;

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
            Debug.Log($"Most frequence Item: {itemTypeMostFrequence}, {countFrequence}");

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
                if (tray1ItemList.Count == 1 || tray2ItemList.Count == 1)
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
                    Debug.Log($"itemListFrequence:  {itemListFrequence.Count}, other: {itemListOther.Count}");

                    if (itemListOther.Count >= 3)
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

                    itemListOther.Sort((itemA, itemB) => itemA.ItemType.CompareTo(itemB.ItemType));
                    tray1.AddRangeItem(itemListFrequence);
                    tray2.AddRangeItem(itemListOther);
                }
            }
        }
        
        if (isShort)
        {
            float tray1Duration = tray1.ShortAndMoveItemToPositionOrDespawn();
            float tray2Duration = tray2.ShortAndMoveItemToPositionOrDespawn();
            return Mathf.Max(tray1Duration, tray2Duration);
        }
        else
        {
            return 0;
        }
        SoundManager.Instance.PlayMergeSound();
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
        if (tray1 == null || tray2 == null) return false;

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
    #endregion

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

        int numberOffItemOnTray = UnityEngine.Random.Range(2, 4); // Random number of items on Tray

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

    //Dat Add
    public Tray CreatePlacedTray(Cell cellPlace)
    {
        Transform newTray = Instantiate(trayPrefab, cellPlace.transform.position, Quaternion.identity);
        if (newTray == null || newTray.GetComponent<Tray>() == null)
        {
            Debug.LogWarning("Developer, please check the prefab configuration & ensure it has a Tray component attached.");
            Debug.LogWarning("TrayManager: trayPrefab don't have Tray component to add |FAIL|");
            return null;
        }

        Tray trayComponent = newTray.GetComponent<Tray>();
        CreateItemOnTray(trayComponent);
        trayComponent.SetCellPlaced(cellPlace);
        cellPlace.SetContainObject(trayComponent);
        return trayComponent;
    }

    //Dat Add
    private void CreateItemOnTray(Tray trayComponent)
    {
        var points = trayComponent.Points;
        var itemTypes = new List<ItemType>();
        trayComponent.ClearItemTraditionalList();

        int numberOffItemOnTray = UnityEngine.Random.Range(2, 4); // Random number of items on Tray

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