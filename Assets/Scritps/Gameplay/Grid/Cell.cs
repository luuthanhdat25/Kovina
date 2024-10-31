using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private Color activeColor;

    private Color defaultColor;
    private SpriteRenderer spriteRenderer;
    private IObject iObject;

    public bool IsContainObject() => iObject != null;

    public IObject GetContainObject() => iObject;

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

    public void SetContainObject(IObject iObject)
    {
        this.iObject = iObject;
    }

    public void ClearContainObject()
    {
        this.iObject = null;
    }

    public Vector2 GetPosition() => transform.position;

    public Vector2 GetScale() => transform.localScale;
}
