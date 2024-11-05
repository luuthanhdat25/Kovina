using Dreamteck;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Level Setup SO", order = 0)]
public class LevelSetUpSO : ScriptableObject
{
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
}

[System.Serializable]
public struct ItemSetUp
{
    public ItemType ItemType;
    public int Number;
}
