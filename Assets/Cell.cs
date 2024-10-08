using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private Color activeColor;

    private Color defaultColor;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    public void ActiveColor(bool isActive)
    {
        spriteRenderer.color = isActive ? activeColor : defaultColor;
    }

    public Vector2 GetPosition() => transform.position;

    public Vector2 GetScale() => transform.localScale;
}
