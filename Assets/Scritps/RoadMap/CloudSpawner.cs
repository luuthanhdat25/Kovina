using UnityEngine;
using UnityEngine.XR;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField]
    private Cloud cloudPrefab;

    [SerializeField]
    private Sprite[] cloudSprites;

    [SerializeField]
    private float halfCloudMoveRange;

    [SerializeField]
    private float maxTimeMove;
    
    [SerializeField]
    private float minTimeMove;

    public float minRanDistance;
    public float maxRanDistance;

    public void SetupCloud(Vector3 spawnPositon, float height)
    {
        float topXCoordinate = spawnPositon.y + (height / 2);
        float botXCoordinate = spawnPositon.y - (height / 2);
        float totalY = botXCoordinate;
        while (totalY < topXCoordinate)
        {
            Cloud cloud = Instantiate(cloudPrefab, this.transform);
            cloud.SetMoveSpeedbyTimeMove(UnityEngine.Random.Range(minTimeMove, maxTimeMove));

            cloud.SetStartAndEndPosion(spawnPositon.x - halfCloudMoveRange, spawnPositon.x + halfCloudMoveRange, totalY);

            SpriteRenderer spriteRenderer = cloud.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];

            var disRan = Random.Range(minRanDistance, maxRanDistance);
            totalY += disRan;
            cloud.transform.localScale = new Vector2(1f/disRan, 1f/disRan);
        }
    }
}
