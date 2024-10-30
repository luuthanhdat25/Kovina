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

        Debug.Log($"Placed Tray {trayComponent.name} in {cellPlaced.name}");
        Tray trayCenter;
        if(cellPlaced.GetContainObject() is Tray tray)
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
        
        // Horizotal First
        List<ItemTraditional> itemHoriList = new List<ItemTraditional>();
        trayHoriList.ForEach(tray => itemHoriList.AddRange(tray.GetItemTraditionalsList()));
        if (IsTrayHaveOneOfRequireItems(trayCenter, itemHoriList))
        {
            //Sort
            if(trayHoriList.Count == 1)
            {
                List<ItemTraditional> trayCenterItemList = trayCenter.GetItemTraditionalsList();
                int numberOfItemInTrayCenter = trayCenterItemList.Count;
                if(numberOfItemInTrayCenter == 1)
                {
                    if(itemHoriList.Count < 3)
                    {
                        var itemMove = trayCenterItemList[0];
                        trayHoriList[0].AddItem(itemMove);
                        trayCenter.RemoveItem(itemMove);
                        trayHoriList[0].ShortAndMoveItemToPosition();
                        cellPlaced.ClearContainObject();
                        //Despawn Tray
                        trayCenter.Despawn(); 
                    }
                    else
                    {
                        int numberOfEqualItem = itemHoriList.Count(item => item.ItemType == trayCenterItemList[0].ItemType);
                        if(numberOfEqualItem == 1)
                        {
                            var itemMove = itemHoriList.FirstOrDefault(item => item.ItemType == trayCenterItemList[0].ItemType);
                            trayCenter.AddItem(itemMove);
                            trayHoriList[0].RemoveItem(itemMove);
                            
                            trayHoriList[0].ShortAndMoveItemToPosition();
                            trayCenter.ShortAndMoveItemToPosition();
                        }
                        else // numberOfEqualItem == 2 
                        {
                            var itemMoveToCenter = itemHoriList.FirstOrDefault(item => item.ItemType != trayCenterItemList[0].ItemType);
                            trayHoriList[0].RemoveItem(itemMoveToCenter);
                            var itemInTrayCenter = trayCenterItemList[0];
                            trayCenter.RemoveItem(itemInTrayCenter);

                            trayHoriList[0].AddItem(itemInTrayCenter);
                            trayCenter.AddItem(itemMoveToCenter);

                            trayCenter.ShortAndMoveItemToPosition();
                            trayHoriList[0].ShortAndMoveItemToPosition();

                            trayHoriList[0].CompletedAndDespawn();
                        }
                    }
                }
                else if(numberOfItemInTrayCenter == 2)
                {
                    
                }
                else
                {

                }
            }
            else // 2 Tray
            {

            }
        }

        /*List<ItemTraditional> itemVerticalList = new List<ItemTraditional>();
        trayVerticalList.ForEach(tray => itemVerticalList.AddRange(tray.GetItemTraditionalsList()));

        if(!IsTrayHaveOneOfRequireItems(trayCenter, itemHorizontalList) && !IsTrayHaveOneOfRequireItems(trayCenter,itemVerticalList ))
        {
            Debug.Log("Can't match!");
            return;
        }*/


        /*itemTraditionals.AddRange(trayCenter.GetItemTraditionalsList());
        if (trayVerticalList.Count == 0)
        {
            itemTraditionals.Sort((a, b) => a.ItemType.CompareTo(b.ItemType));
            string r = "[TrayManager] Debug List Item =>[";
            foreach (var item in itemTraditionals)
            {
                r += item.ItemType.ToString() + "-";
            }
            Debug.Log(r + "]");
        }*/

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

    private bool IsTrayHaveOneOfRequireItems(Tray tray, List<ItemTraditional> requireItems)
    {
        foreach (var item in tray.GetItemTraditionalsList())
        {
            var existItem = requireItems.FirstOrDefault(reItem => reItem.ItemType == item.ItemType);
            if (existItem != null) return true;
        }
        return false;
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
        //int numberOffItemOnTray = UnityEngine.Random.Range(1, points.Count + 1); // Random number of item on Tray
        int numberOffItemOnTray = UnityEngine.Random.Range(1, 3); // Random number of item on Tray

        for (int i = 0; numberOffItemOnTray > 0; i++, numberOffItemOnTray--)
        {
            var itemType = ItemTraditionalManager.Instance.Spawner.GetRandomEnumValue();
            var item = ItemTraditionalManager.Instance.Spawner.Spawn(itemType);

            trayComponent.AddItem(item);
            item.transform.SetParent(trayComponent.transform);
            item.transform.position = points[i].position;
            item.transform.localScale = points[i].localScale;
            itemTypes.Add(itemType);
        }
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
