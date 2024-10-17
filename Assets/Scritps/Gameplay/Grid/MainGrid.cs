using UnityEngine;

public class MainGrid : MonoBehaviour
{
    [SerializeField]
    private Cell cellPrefab;

    [SerializeField]
    private string levelLoadPath = "Level/Level_Grid_";

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

    void Start()
    {
        LevelGridSO levelGridData = Resources.Load<LevelGridSO>(levelLoadPath + levelNumber);
        if(levelGridData != null)
        {
            InitialGrid(levelGridData);
        }
        else
        {
            Debug.LogWarning("File" + levelLoadPath + levelNumber + " doesn't exist!");
        }
    }

    private void InitialGrid(LevelGridSO levelGridSO)
    {
        InitialCells(levelGridSO);

        InitialBoxs(levelGridSO);
    }

    private void InitialCells(LevelGridSO levelGridSO)
    {
        int widthSize = levelGridSO.WidthSize;
        int heightSize = levelGridSO.HeighSize;

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

    private void InitialBoxs(LevelGridSO levelGridSO)
    {
        foreach (var box in levelGridSO.BoxSetups)
        {
            if (IsInGrid(box.XPosition, box.YPosition, levelGridSO))
            {
                Cell cell = cellArray[box.XPosition, box.YPosition];
                if (!cell.IsContainObject)
                {
                    switch (box.Type)
                    {
                        case BoxType.QuestionBox:
                            QuestionBox questionBox = Instantiate(questionBoxPrefab, transform);
                            questionBox.transform.position = cellArray[box.XPosition, box.YPosition].transform.position;
                            break;

                        case BoxType.ExplosiveBox:
                            ExplosiveBox explosiveBox = Instantiate(explosiveBoxPrefab, transform);
                            explosiveBox.transform.position = cellArray[box.XPosition, box.YPosition].transform.position;
                            explosiveBox.SetCellPlace(cell);
                            break;
                    }
                    cell.SetContainObjectTrue();
                }
            }
        }
    }

    private Vector3 GetGirdCenterPosition()
    {
        Vector3 newPos = centerRect.position * CameraManager.Instance.ZoomRatio();
        return newPos;
    }

    private bool IsInGrid(int x, int y, LevelGridSO levelGridSO)
    {
        if (x < 0 || y < 0) return false;
        if (x >= levelGridSO.WidthSize || y >= levelGridSO.HeighSize) return false;
        return true;
    }
}