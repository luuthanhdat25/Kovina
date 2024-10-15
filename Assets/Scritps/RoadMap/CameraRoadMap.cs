using System.Collections;
using UnityEngine;

public class CameraRoadMap : MonoBehaviour
{
    [SerializeField]
    private BackgroundRoadmapGenerater backgroundRoadmap;

    [SerializeField]
    private float swipeThreshold = 1.0f;

    [SerializeField]
    private float swipeForceMultiplier = 1.0f;

    [SerializeField] 
    private float friction = 0.95f;

    private Vector3 startTouchPosition, endTouchPosition;
    private float swipeStartTime, swipeEndTime;
    private new Camera camera;
    private float minLimitX, maxLimixX;
    private Vector2 swipeVelocityVector;

    private void Awake() => camera = GetComponent<Camera>();

    private void Start() => SetCameraToRoadmap();

    #region Setup Camera
    // Set the camera based on the roadmap size and position
    private void SetCameraToRoadmap()
    {
        Vector2 startPos = backgroundRoadmap.GetStartPosition();
        Vector2 endPos = backgroundRoadmap.GetEndPosition();
        Vector2 endMap1Pos = backgroundRoadmap.GetEndMap1Position();

        var witdthCameraScale = endMap1Pos.x - startPos.x;

        // Set camera size
        camera.orthographicSize = witdthCameraScale / (2 * camera.aspect);

        float cameraHalfWidth = witdthCameraScale / 2;
        var heightCameraScale = witdthCameraScale / camera.aspect;
        Debug.Log($"Camera width and Height: {witdthCameraScale}, {heightCameraScale}");


        Vector3 cameraPosition = transform.position;
        Vector2 cameraScaleWorldSpace = new Vector2(witdthCameraScale, heightCameraScale);

        backgroundRoadmap.SetupAllBackground(cameraScaleWorldSpace, camera.aspect);
        
        minLimitX = startPos.x + cameraHalfWidth;
        maxLimixX = endPos.x - cameraHalfWidth;

        cameraPosition.x = minLimitX;
        transform.position = cameraPosition;
    }
    #endregion

    #region Input Swipe
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = ScreenToWorldPosition(touch.position);
                    swipeStartTime = Time.time;
                    swipeVelocityVector = Vector2.zero;
                    break;

                case TouchPhase.Moved:
                    Vector3 currentPosition = ScreenToWorldPosition(touch.position);
                    Vector3 translate = startTouchPosition - currentPosition;
                    translate.y = 0;

                    transform.Translate(translate);
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = ScreenToWorldPosition(touch.position);
                    swipeEndTime = Time.time;
                    float swipeDuration = swipeEndTime - swipeStartTime;

                    Vector2 swipeDirection = startTouchPosition - endTouchPosition;
                    float swipeVelocity = swipeDirection.magnitude / swipeDuration;

                    if (swipeVelocity > swipeThreshold)
                    {
                        swipeDirection.y = 0;
                        swipeVelocityVector = swipeDirection * swipeForceMultiplier;
                    }
                    else
                    {
                        //Debug.Log("Swipe to small");
                    }

                    //Debug.Log("Swipe velocity: " + swipeVelocity);
                    break;
            }
        }

        if(swipeVelocityVector != Vector2.zero)
        {
            transform.Translate(swipeVelocityVector * Time.deltaTime);
            swipeVelocityVector *= friction;

            if (swipeVelocityVector.magnitude < 0.7f)
            {
                swipeVelocityVector = Vector2.zero; 
            }
        }
    }

    // Clamp camera position alway belong to limit
    private void LateUpdate()
    {
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(transform.position.x, minLimitX, maxLimixX);
        transform.position = currentPos;
    }

    private Vector3 ScreenToWorldPosition(Vector2 touchPosition)
    {
        return camera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, camera.nearClipPlane));
    }
    #endregion
}
