using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum ItemType
{
    //Korean traditional foods
    Korean_SushiRoll,
    Korean_Kimchi,
    Korean_Soju,
    Korean_GrillMeat,
}

public class ItemTraditionalSpawner : ObjectPooling<ItemTraditional, ItemType>
{
    [SerializeField]
    private string levelLoadPath = "Level/Level_";

    private ItemType[] itemTypes;

    private void LoadItemType()
    {
        LoadScene loadLevel = LoadScene.Instance;
        int levelNumber;
        if (loadLevel != null)
        {
            levelNumber = loadLevel.Level <= 0 ? 1 : loadLevel.Level;
        }
        else
        {
            levelNumber = 1;
        }

        LevelSetUpSO levelGridData = Resources.Load<LevelSetUpSO>(levelLoadPath + levelNumber);

        if (levelGridData != null)
        {
            if(levelGridData.ItemTypes.Length < 2)
            {
                Debug.LogError("[ItemTraditionalSpawner] Number of ItemType can't less than 2");
            }
            itemTypes = levelGridData.ItemTypes;
        }
        else
        {
            Debug.LogWarning("[ItemTraditionalSpawner] " + "File" + levelLoadPath + levelNumber + " doesn't exist!");
        }
    }

    public Sprite GetSpriteForItem(ItemType itemType)
    {
        var matchedItem = componentPrefabs.FirstOrDefault(item => CheckMatchValue(itemType, item));

        if (matchedItem != null)
        {
            var spriteRenderer = matchedItem.GetComponent<SpriteRenderer>();
            return spriteRenderer?.sprite;
        }

        Debug.LogWarning($"[ItemTraditionalSpawner] GetSpriteForItem: No prefab found for {itemType} | NULL |");
        return null;
    }

    public List<Sprite> GetSpritesForItems(List<ItemType> itemTypes)
    {
        return itemTypes.Select(GetSpriteForItem).ToList();
    }

    protected override bool CheckMatchValue(ItemType matchType, ItemTraditional component)
    {
        //Debug.Log($"[ItemTraditionalSpawner] CheckMatchValue: {component.ItemType} == {matchType}");
        return component.ItemType == matchType;
    }

    public override ItemType GetRandomEnumValue()
    {
        if (itemTypes == null) LoadItemType();
        return itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
    }
}
