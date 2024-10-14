using UnityEngine;

public class BackgroundRoadmap : MonoBehaviour
{
    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint;
    
    public Vector2 GetStartPosition() => startPoint.position;

    public Vector2 GetEndPosition() => endPoint.position;

    public float GetStartEndPointDistance() => endPoint.position.x - startPoint.position.x;

    public void SetScale(Vector3 scale) => transform.localScale = scale;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

    public void SetPosition(Vector3 position) => transform.position = position;

    public Sprite SpriteBackground => spriteRenderer.sprite;
}
