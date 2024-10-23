using System;
using System.Collections;
using System.Collections.Generic;
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

    public void OnTrayPlaced()
    {
        countTrayPlaced = countTrayPlaced + 1;
        if (countTrayPlaced >= TRAY_UI_MAXIMUM)
        {
            countTrayPlaced = 0;
            OnEnoughTrayPlaced?.Invoke();
            trayUIContainer.OnEnableTraysUI();
        }
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

        foreach (Transform point in points)
        {
            var itemType = ItemTraditionalManager.Instance.Spawner.GetRandomEnumValue();
            var item = ItemTraditionalManager.Instance.Spawner.Spawn(itemType);

            item.transform.SetParent(trayComponent.transform);
            item.transform.position = point.position;
            item.transform.localScale = point.localScale;
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
