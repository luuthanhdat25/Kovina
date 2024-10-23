using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private Color activeColor;

    private Color defaultColor;
    private SpriteRenderer spriteRenderer;
    private bool isContainObject = false;
    public bool IsContainObject => isContainObject;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    public void ActiveSelectVisual()
    {
        spriteRenderer.color = activeColor;
    }

    public void UnActiveSelectVisual()
    {
        spriteRenderer.color = defaultColor;
    }

    public void SetContainObjectTrue()
    {
        isContainObject = true;
    }

    public void SetContainObjectFalse()
    {
        isContainObject = false;
    }

    public Vector2 GetPosition() => transform.position;

    public Vector2 GetScale() => transform.localScale;
}
