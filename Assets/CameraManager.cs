using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Camera camera;
    public Camera Camera => camera;

    [SerializeField] 
    private float heightPaddingMultipler = 1.2f;
    
    [SerializeField]
    private float widthPaddingMultipler = 1.5f;

    private float defaultCameraSize;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        defaultCameraSize = camera.orthographicSize;
    }

    public void SetSize(float gridHeight, float gridWidth, int heightSize,int widthSize)
    {
        float size;
        if(widthSize >= heightSize)
        {
            size = gridWidth / (2 * camera.aspect);
            size *= widthPaddingMultipler;
        }
        else
        {
            size = gridHeight / 2;
            size *= heightPaddingMultipler;
        }
        camera.orthographicSize = size;
    }

    public float ZoomRatio() => camera.orthographicSize / defaultCameraSize;
}
