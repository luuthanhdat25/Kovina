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

        if (widthSize >= heightSize)
        {
            size = gridWidth / (2 * aspectRatio);
            size *= widthPaddingMultipler;
        }
        else
        {
            size = gridHeight / 2;
            size *= heightPaddingMultipler;
        }
        Debug.Log("Ratio: " + aspectRatio);
        if (aspectRatio >= 1.7f && aspectRatio < 2) // Tỷ lệ này thường cho điện thoại (16:9 hoặc lớn hơn)
        {
            size *= ratio16_9Multipler; // Điều chỉnh zoom một chút cho điện thoại
        }
        else if (aspectRatio < 1.5f) // Tỷ lệ này thường cho iPad (4:3)
        {
            size *= ratio4_3Multipler; // Điều chỉnh zoom cho iPad
        }

        camera.orthographicSize = size;
    }


    public float ZoomRatio() => camera.orthographicSize / defaultCameraSize;
}
