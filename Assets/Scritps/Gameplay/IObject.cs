using UnityEngine;

/// <summary>
/// Define a Object in Cell
/// </summary>
public interface IObject {
    public float DoAction();
    public float DoAction(Cell cellPlaced);
}
