using System.Collections.Generic;
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
        return LoadScene.Instance.LevelSetUpSO.GetRandomItemType();
    }
}
