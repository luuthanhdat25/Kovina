using System;
using UnityEngine;

[Serializable]
public class ItemTraditional: MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Tray tray;
    [SerializeField] private ItemType itemType = ItemType.Korean_GrillMeat;

    public int ID => id;
    public Tray Tray => tray;
    public ItemType ItemType => itemType;
        
    public void Init(int id, Tray tray, ItemType itemType)
    {
        this.id = id;
        this.tray = tray;
        this.itemType = itemType;
    }
}
