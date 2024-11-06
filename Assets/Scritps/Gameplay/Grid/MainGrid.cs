using System.Collections.Generic;
using UnityEngine;

public class MainGrid : Singleton<MainGrid>
{
    [SerializeField]
    private Cell cellPrefab;

    [SerializeField]
    private int levelNumber;

    [Header("[Cell Padding]")]
    [SerializeField]
    private float widthPadding;

    [SerializeField]
    private float heightPadding;

    [SerializeField]
    private RectTransform centerRect;

    [SerializeField]
    private QuestionBox questionBoxPrefab;

    [SerializeField]
    private ExplosiveBox explosiveBoxPrefab;

    private Cell[,] cellArray;
    private int widthSize;
    private int heightSize;

    private void Start()
    {
        LevelSetUpSO levelGridData = LoadScene.Instance.LevelSetUpSO;
        InitialGrid(levelGridData);
    }

    private void InitialGrid(LevelSetUpSO levelGridSO)
    {
        InitialCells(levelGridSO);

        InitialBoxs(levelGridSO);
    }

    private void InitialCells(LevelSetUpSO levelGridSO)
    {
        widthSize = levelGridSO.WidthSize;
        heightSize = levelGridSO.HeighSize;

        cellArray = new Cell[widthSize, heightSize];
        Vector2 cellScale = cellPrefab.GetScale();

        float gridWidth = (widthSize * cellScale.x) + ((widthSize - 1) * widthPadding);
        float gridHeight = (heightSize * cellScale.y) + ((heightSize - 1) * heightPadding);
        CameraManager.Instance.SetSize(gridHeight, gridWidth);

        Vector3 girdCenterPosition = GetGirdCenterPosition();

        Vector3 startPos = new Vector2(girdCenterPosition.x - gridWidth / 2, girdCenterPosition.y - gridHeight / 2) + cellScale / 2;

        for (int i = 0; i < widthSize; i++)
        {
            for (int j = 0; j < heightSize; j++)
            {
                Cell newCell = Instantiate(cellPrefab);
                newCell.transform.SetParent(transform);
                newCell.name = $"Cell {i}, {j}";

                float posX = startPos.x + i * (widthPadding + cellScale.x);
                float posY = startPos.y + j * (heightPadding + cellScale.y);
                newCell.transform.position = new Vector2(posX, posY);

                cellArray[i, j] = newCell;
            }
        }
    }

    private void InitialBoxs(LevelSetUpSO levelGridSO)
    {
        foreach (var box in levelGridSO.BoxSetups)
        {
            if (IsInGrid(box.XPosition, box.YPosition))
            {
                Cell cell = cellArray[box.XPosition, box.YPosition];
                if (!cell.IsContainObject())
                {
                    IObject iObject = null;
                    switch (box.Type)
                    {
                        case BoxType.QuestionBox:
                            QuestionBox questionBox = Instantiate(questionBoxPrefab, transform);
                            iObject = questionBox;
                            questionBox.transform.position = cellArray[box.XPosition, box.YPosition].transform.position;
                            break;

                        case BoxType.ExplosiveBox:
                            ExplosiveBox explosiveBox = Instantiate(explosiveBoxPrefab, transform);
                            iObject = explosiveBox;
                            explosiveBox.transform.position = cellArray[box.XPosition, box.YPosition].transform.position;
                            break;
                    }
                    cell.SetContainObject(iObject);
                }
            }
            else
            {
                Debug.LogError($"{box.Type} at coordinate: {box.XPosition},{box.YPosition} is out of Grid!");
            }
        }
    }

    private Vector3 GetGirdCenterPosition()
    {
        Vector3 newPos = centerRect.position * CameraManager.Instance.ZoomRatio();
        return newPos;
    }

    private bool IsInGrid(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= widthSize || y >= heightSize) return false;
        return true;
    }

    public List<Cell> GetCellsHorizontal(Cell centerCell)
    {
        List<Cell> cellList = new List<Cell>();

        (int x, int y) = GetCellCoordinates(centerCell);

        if (x == -1 || y == -1)
        {
            Debug.LogError("[MainGrid] Center cell is not part of the grid.");
            return cellList;
        }

        if (IsInGrid(x - 1, y)) cellList.Add(cellArray[x - 1, y]);
        //cellList.Add(centerCell);
        if (IsInGrid(x + 1, y)) cellList.Add(cellArray[x + 1, y]);
        return cellList;
    }

    public List<Cell> GetCellsVertical(Cell centerCell)
    {
        List<Cell> cellList = new List<Cell>();

        (int x, int y) = GetCellCoordinates(centerCell);

        if (x == -1 || y == -1)
        {
            Debug.LogError("[MainGrid] Center cell is not part of the grid.");
            return cellList;
        }

        if (IsInGrid(x, y + 1)) cellList.Add(cellArray[x, y + 1]);
        //cellList.Add(centerCell);
        if (IsInGrid(x, y - 1)) cellList.Add(cellArray[x, y - 1]);
        return cellList;
    }


    public (int, int) GetCellCoordinates(Cell targetCell)
    {
        for (int x = 0; x < widthSize; x++)
        {
            for (int y = 0; y < heightSize; y++)
            {
                if (cellArray[x, y] == targetCell)
                {
                    return (x, y);
                }
            }
        }

        return (-1, -1);
    }

    public void ClearTrayInCell(Tray tray)
    {
        for (int x = 0; x < widthSize; x++)
        {
            for (int y = 0; y < heightSize; y++)
            {
                if (cellArray[x, y].GetContainObject() == tray as IObject)
                {
                    cellArray[x, y].ClearContainObject();
                }
            }
        }
    }
}