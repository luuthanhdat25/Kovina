using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private new Camera camera;
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

    // Set the camera size based on the grid dimensions and aspect ratio
    public void SetSize(float gridHeight, float gridWidth)
    {
        float size;
        float aspectRatio = camera.aspect;

        var areaUiWidthPerHeight = gridAreaRect.rect.width / gridAreaRect.rect.height;
        var gridWithDivHeight = gridWidth / gridHeight;
        if (gridWidth >= gridHeight && areaUiWidthPerHeight < gridWithDivHeight)
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

        camera.orthographicSize = size;
    }


    public float ZoomRatio() => camera.orthographicSize / defaultCameraSize;
}