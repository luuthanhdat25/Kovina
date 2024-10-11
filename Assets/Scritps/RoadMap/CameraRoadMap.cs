using System.Collections;
using UnityEngine;

public class CameraRoadMap : MonoBehaviour
{
    [SerializeField]
    private BackgroundRoadmap backgroundRoadmap;

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
        Vector2 size = backgroundRoadmap.GetBackroundSize();

        Debug.Log($"Start bg Pos: {startPos}, End bg Pos: {endPos}, Size bg: {size}");

        // Set camera size
        camera.orthographicSize = size.y / 2;

        float cameraHalfWidth = (size.y * camera.aspect) / 2;

        Vector3 cameraPosition = transform.position;
        minLimitX = startPos.x + cameraHalfWidth;
        maxLimixX = endPos.x - cameraHalfWidth;

        cameraPosition.x = minLimitX;
        transform.position = cameraPosition;
    }
    #endregion

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
                        Debug.Log("Swipe to small");
                    }

                    Debug.Log("Swipe velocity: " + swipeVelocity);
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
}
