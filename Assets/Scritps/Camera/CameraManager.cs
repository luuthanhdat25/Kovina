using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera camera;
    public Camera Camera => camera;

    [SerializeField] 
    private float heightPaddingMultipler = 1.2f;
    
    [SerializeField]
    private float widthPaddingMultipler = 1.5f;

    [SerializeField]
    private float ratio16_9Multipler = 1f;

    [SerializeField]
    private float ratio4_3Multipler = 1.2f;

    [SerializeField]
    private RectTransform canvasRect, gridAreaRect, leftRect, rightRect;

    private float defaultCameraSize;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        defaultCameraSize = camera.orthographicSize;
    }

    public void SetSize(float gridHeight, float gridWidth, int heightSize, int widthSize)
    {
        float size;
        float aspectRatio = camera.aspect;
        float canvas = canvasRect.rect.width;
        float gridArea = gridAreaRect.rect.width;
        //float hori = Vector3.Distance(leftRect.position, rightRect.position);
        Debug.Log($"canvas: {canvas}, gridarea: {gridArea}");
        var r = canvas / gridArea;
        Debug.Log(canvas/gridArea);
        if (gridWidth >= gridHeight)
        {
            size = gridWidth / (2 * aspectRatio);
            size *= r;
            //var r = hori / size;
            //size /= r;
        }
        else
        {
            size = gridHeight / 2;
        }
        Debug.Log("Ratio: " + aspectRatio);
        

        //Debug.Log($"Hori/with: {hori / gridWidth}");
        /*if (aspectRatio >= 1.7f && aspectRatio < 2) // Tỷ lệ này thường cho điện thoại (16:9 hoặc lớn hơn)
        {
            size *= ratio16_9Multipler; // Điều chỉnh zoom một chút cho điện thoại
        }
        else if (aspectRatio < 1.5f) // Tỷ lệ này thường cho iPad (4:3)
        {
            size *= ratio4_3Multipler; // Điều chỉnh zoom cho iPad
        }*/

        camera.orthographicSize = size;
    }


    public float ZoomRatio() => camera.orthographicSize / defaultCameraSize;
}
