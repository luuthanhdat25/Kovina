using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Level Setup SO", order = 0)]
public class LevelSetUpSO : ScriptableObject
{
    [SerializeField]
    private int timePlay = 180;

    [SerializeField]
    private ItemSetUp[] itemSetUp;

    [Header("[Cell size]")]
    [SerializeField]
    private int widthSize;

    [SerializeField]
    private int heightSize;

    [Space]
    [SerializeField]
    private BoxSetUp[] boxSetUps;

    public int WidthSize => widthSize;
    public int HeighSize => heightSize;
    public BoxSetUp[] BoxSetups => boxSetUps;
    public ItemType[] ItemTypes => itemSetUp.Select(item => item.ItemType).ToArray();
    public ItemSetUp[] ItemSetUp => itemSetUp;
    public int TimePlay => timePlay;

    public ItemType GetRandomItemType()
    {
        if (itemSetUp == null || itemSetUp.Length == 0)
        {
            Debug.LogWarning("ItemSetUp array is empty or null.");
            return default;
        }

        int randomIndex = Random.Range(0, itemSetUp.Length);
        return itemSetUp[randomIndex].ItemType;
    }
}

[System.Serializable]
public struct ItemSetUp
{
    public ItemType ItemType;
    public int Number;
}
