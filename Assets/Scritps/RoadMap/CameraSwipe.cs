using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwipe : MonoBehaviour
{
    

    [SerializeField]
    private float swipeSpeed = 1;

    private Vector3 startTouchPosition, endTouchPosition;
    private float swipeStartTime, swipeEndTime;
    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }


    

    /*private void DetectSwipe()
    {
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;
        float swipeDuration = swipeEndTime - swipeStartTime;
        float swipeVelocity = swipeDirection.magnitude / swipeDuration;

        // Xử lý hướng swipe
        if (swipeDirection.x > Mathf.Abs(swipeDirection.y))
            Debug.Log("Swipe Right");
        else if (swipeDirection.x < -Mathf.Abs(swipeDirection.y))
            Debug.Log("Swipe Left");
        else if (swipeDirection.y > Mathf.Abs(swipeDirection.x))
            Debug.Log("Swipe Up");
        else if (swipeDirection.y < -Mathf.Abs(swipeDirection.x))
            Debug.Log("Swipe Down");

        // Xử lý vận tốc swipe
        //Debug.Log("Swipe velocity: " + swipeVelocity);
        //rigidbody2d.velocity = swipeDirection * swipeSpeed;
    }*/

    
}
