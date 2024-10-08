using UnityEngine;

public class MainGrid : Singleton<MainGrid>
{
    [SerializeField]
    private Cell cellPrefab;

    [Header("[Cell size]")]
    [SerializeField]
    private int widthSize;

    [SerializeField]
    private int heightSize;

    [Header("[Padding]")]
    [SerializeField]
    private float widthPadding;

    [SerializeField]
    private float heightPadding;

    private Cell[,] cellArray;

    void Start()
    {
        cellArray = new Cell[heightSize, widthSize];
        Vector2 cellScale = cellPrefab.GetScale();

        float gridWidth = (widthSize * cellScale.x) + ((widthSize - 1) * widthPadding);
        float gridHeight = (heightSize * cellScale.y) + ((heightSize - 1) * heightPadding);

        Vector2 girdCenterPosition = GetGirdCenterPosition();

        Vector2 startPos = new Vector2(girdCenterPosition.x - gridWidth / 2, girdCenterPosition.y - gridHeight / 2) + cellScale / 2;

        for (int i = 0; i < heightSize; i++)
        {
            for (int j = 0; j < widthSize; j++)
            {
                Cell newCell = Instantiate(cellPrefab);
                newCell.transform.SetParent(transform);

                float posX = startPos.x + j * (widthPadding + cellScale.x);
                float posY = startPos.y + i * (heightPadding + cellScale.y);
                newCell.transform.position = new Vector2(posX, posY);

                cellArray[i, j] = newCell;
            }
        }
    }

    private Vector3 GetGirdCenterPosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
    }

    private void TraceCell(Vector2 position)
    {

    }

    /*private Cell GetCellByPosition(Vector2 position)
    {

    }*/
}
