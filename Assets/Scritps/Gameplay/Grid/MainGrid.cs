using UnityEngine;

public class MainGrid : MonoBehaviour
{
    public enum BoxType
    {
        QuestionBox,
        ExplosiveBox
    }

    [System.Serializable]
    public struct Box
    {
        public BoxType Type;
        public int XPosition;
        public int YPosition;
    }

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

    [SerializeField]
    private QuestionBox questionBoxPrefab;

    [SerializeField]
    private ExplosiveBox explosiveBoxPrefab;

    [SerializeField]
    private Box[] boxSetUps;

    private Cell[,] cellArray;

    void Start()
    {
        InitialGrid();
    }

    private void InitialGrid()
    {
        InitialCells();

        InitialBoxs();
    }

    private void InitialCells()
    {
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

    private void InitialBoxs()
    {
        foreach (var box in boxSetUps)
        {
            if (IsInGrid(box.XPosition, box.YPosition))
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

    private bool IsInGrid(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= widthSize || y >= heightSize) return false;
        return true;
    }
}