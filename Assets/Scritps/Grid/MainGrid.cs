using Crystal;
using System;
using System.Drawing;
using UnityEngine;

public class MainGrid : MonoBehaviour
{
    [SerializeField]
    private Cell cellPrefab;

    [Header("[Cell size]")]
    [SerializeField]
    private int widthSize;

    [SerializeField]
    private int heightSize;

    [Header("[Cell Padding]")]
    [SerializeField]
    private float widthPadding;

    [SerializeField]
    private float heightPadding;

    [SerializeField]
    private RectTransform centerRect;

    private Cell[,] cellArray;

    void Start()
    {
        InitialGrid();
    }

    private void InitialGrid()
    {
        cellArray = new Cell[heightSize, widthSize];
        Vector2 cellScale = cellPrefab.GetScale();

        float gridWidth = (widthSize * cellScale.x) + ((widthSize - 1) * widthPadding);
        float gridHeight = (heightSize * cellScale.y) + ((heightSize - 1) * heightPadding);
        Debug.Log($"width: {gridWidth}, gridHeight: {gridHeight}");
        CameraManager.Instance.SetSize(gridHeight, gridWidth, heightSize, widthSize);

        Vector3 girdCenterPosition = GetGirdCenterPosition();

        Vector3 startPos = new Vector2(girdCenterPosition.x - gridWidth / 2, girdCenterPosition.y - gridHeight / 2) + cellScale / 2;

        for (int i = 0; i < heightSize; i++)
        {
            for (int j = 0; j < widthSize; j++)
            {
                Cell newCell = Instantiate(cellPrefab);
                newCell.transform.SetParent(transform);
                newCell.name = $"Cell {i}, {j}";

                float posX = startPos.x + j * (widthPadding + cellScale.x);
                float posY = startPos.y + i * (heightPadding + cellScale.y);
                newCell.transform.position = new Vector2(posX, posY);

                cellArray[i, j] = newCell;
            }
        }
    }

    private Vector3 GetGirdCenterPosition()
    {
        Vector3 newPos = centerRect.position * CameraManager.Instance.ZoomRatio();
        newPos.y = CameraManager.Instance.Camera.transform.position.y;
        return newPos;
    }
}