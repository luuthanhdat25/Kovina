using System.Collections.Generic;
using UnityEngine;

public class BackgroundRoadmap : MonoBehaviour
{
    [SerializeField]
    private List<Transform> backgroundList;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint;

    public Vector2 GetStartPosition() => startPoint.position;

    public Vector2 GetEndPosition() => endPoint.position;

    public Vector2 GetBackroundSize()
    {
        float y = backgroundList[0].localScale.y;
        float x = 0;
        foreach (Transform background in backgroundList)
        {
            x += background.localScale.x;
        }
        return new Vector2 (x, y);
    } 
}
