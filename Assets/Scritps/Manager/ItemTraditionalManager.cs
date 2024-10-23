using UnityEngine;

public class ItemTraditionalManager : Singleton<ItemTraditionalManager>
{
    [SerializeField] private ItemTraditionalSpawner spawner;
    public ItemTraditionalSpawner Spawner => spawner;
}
