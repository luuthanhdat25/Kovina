using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayManager : Singleton<TrayManager>
{
    [Header("Components"), Space(6)]
    [SerializeField] private Transform trayPrefab;
    [SerializeField] private Transform trayHolder;

    private List<Tray> trayList = new List<Tray>();
    private Tray trayPresent;

    private void Start()
    {
        CreateTray(new Vector3(1000, 0, 0)).gameObject.SetActive(false);
        CreateTray(new Vector3(1000, 0, 0)).gameObject.SetActive(false);
        CreateTray(new Vector3(1000, 0, 0)).gameObject.SetActive(false);
    }

    #region CRUD

    //CREATE -------------------------------------------------------------
    public Transform CreateTray(Vector3 trayPosition = default(Vector3), Quaternion quaternion = default(Quaternion))
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
        AddNewTrayToList(trayComponent);

        newTray.SetParent(trayHolder);
        return newTray;
    }

    private void AddNewTrayToList(Tray trayComponent)
    {
        if (trayList == null)
        {
            Debug.Log("TrayManager: trayList has not been initialized. Initializing now...");
            trayList = new List<Tray>();
        }
        
        trayList.Add(trayComponent);
        Debug.Log($"TrayManager: add {trayComponent} element to trayList |SUCCESS|");
    }

    //READ ----------------------------------------------------------------
    public Tray GetTray(int index)
    {
        if (index < 0 || index >= trayList.Count)
        {
            Debug.LogWarning($"Developer, Please ensure the index is within the range (0 to {trayList.Count - 1})");
            Debug.LogWarning($"TrayManager: Invalid index {index}.");
            return null;
        }
        return trayList[index];
    }

    //UPDATE --------------------------------------------------------------
    public void EnableTray(int index) => ChangeTrayStatusEnable(index, true);
    public void DisableTray(int index) => ChangeTrayStatusEnable(index, false);

    private void ChangeTrayStatusEnable(int index, bool status)
    {
        Tray trayComponent = GetTray(index);
        if (trayComponent == null)
        {
            Debug.LogWarning($"TrayManager: Tray at index {index} is null or doesn't exist |FAIL|");
            return;
        }
        trayComponent.gameObject.SetActive(status);
    }

    #endregion
}
