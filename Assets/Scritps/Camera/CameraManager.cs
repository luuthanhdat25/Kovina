using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera camera;
    public Camera Camera => camera;

    [SerializeField]
    private float heightPaddingMultipler = 1.2f;

    [SerializeField]
    private float widthPaddingMultipler = 1.2f;

    [SerializeField]
    private RectTransform canvasRect, gridAreaRect;

    private float defaultCameraSize;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        defaultCameraSize = camera.orthographicSize;
    }

    public void SetSize(float gridHeight, float gridWidth)
    {
        float size;
        float aspectRatio = camera.aspect;
        Debug.Log($"width: {gridWidth}, height: {gridHeight}");

        //Debug.Log($"GridGameObject: {gridWidth / gridHeight}");
        //Debug.Log($"GridUI: {gridAreaRect.rect.width / gridAreaRect.rect.height}");
        //Debug.Log($"hori: {canvasRect.rect.width / gridAreaRect.rect.width}");
        //Debug.Log($"verti: {canvasRect.rect.height / gridAreaRect.rect.height}");
        var perUI = gridAreaRect.rect.width / gridAreaRect.rect.height;
        var grid = gridWidth / gridHeight;
        Debug.Log($"perUI: {perUI}, grid: {grid}");
        if (gridWidth >= gridHeight && perUI < grid)
        {
            size = gridWidth / (2 * aspectRatio);
            var canvasDivGridAreaWidth = canvasRect.rect.width / gridAreaRect.rect.width;
            size *= canvasDivGridAreaWidth;
            size *= widthPaddingMultipler;
        }
        else
        {
            size = gridHeight / 2;
            var canvasDivGridAreaHeigh = canvasRect.rect.height / gridAreaRect.rect.height;
            size *= canvasDivGridAreaHeigh;
            size *= heightPaddingMultipler;
        }

        Debug.Log("Ratio: " + aspectRatio);

        camera.orthographicSize = size;
    }


    public float ZoomRatio() => camera.orthographicSize / defaultCameraSize;
}