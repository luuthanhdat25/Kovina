using UnityEngine;

[CreateAssetMenu(fileName = "Level_Grid_", menuName = "Level Grid SO", order = 0)]
public class LevelGridSO : ScriptableObject
{
    [Header("[Cell size]")]
    [SerializeField]
    private int widthSize;

    [SerializeField]
    private int heightSize;

    [Space]
    [SerializeField]
    private Box[] boxSetUps;

    public int WidthSize => widthSize;
    public int HeighSize => heightSize;
    public Box[] BoxSetups => boxSetUps;
}
