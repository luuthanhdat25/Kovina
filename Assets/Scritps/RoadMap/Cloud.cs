using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private bool movingToEnd;
    private float moveProgress;
    private float moveSpeed;
    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        moveProgress = UnityEngine.Random.Range(0f, 1f);
        movingToEnd = UnityEngine.Random.Range(0f, 1f) > 0.5f;
    }

    public void SetStartAndEndPosion(float xStart, float xEnd, float yCoordinate)
    {
        startPosition = new Vector3(xStart, yCoordinate, transform.position.z);
        endPosition = new Vector3(xEnd, yCoordinate, transform.position.z);
    }

    public void SetMoveSpeedbyTimeMove(float timeMove) => moveSpeed = 1f / timeMove;

    void FixedUpdate()
    {
        MoveCloud();
    }

    private void MoveCloud()
    {
        moveProgress += (movingToEnd ? 1 : -1) * Time.fixedDeltaTime * moveSpeed;

        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(moveProgress));

        if (moveProgress >= 1f || moveProgress <= 0f)
        {
            movingToEnd = !movingToEnd;
        }
    }
}
