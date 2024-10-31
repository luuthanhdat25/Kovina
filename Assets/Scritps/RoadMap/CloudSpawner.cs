using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField]
    private Cloud cloudPrefab;

    [SerializeField]
    private float halfCloudMoveRange;

    public void SpawnCloudParticle(Vector3 spawnPositon, float height)
    {
        Cloud cloud = Instantiate(cloudPrefab, this.transform);
        cloud.SetPosition(spawnPositon);
        cloud.SetScale(new Vector3(halfCloudMoveRange * 2, height, 1));
    }
}
