using UnityEngine;

public class ExplosiveBox : MonoBehaviour
{
    [SerializeField]
    private float scaleInDuration = .5f;

    [SerializeField]
    private float scaleOutDuration = 1f;

    [SerializeField]
    private float scaleOutSizeMultiplier = 1.5f;

    [SerializeField]
    private ParticleSystem explosionEffect;

    [SerializeField]
    private float delaySpawnEffect = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Cell cellPlace;

    public void SetCellPlace(Cell cell) => cellPlace = cell;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        Explode();
    }

    public void Explode()
    {
        cellPlace.SetContainObjectFalse();
        DoAnimation();
    }

    private void DoAnimation()
    {
        Color spriteColor = spriteRenderer.color;

        LeanTween.scale(gameObject, scaleOutSizeMultiplier * transform.localScale, scaleOutDuration)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(gameObject, Vector2.zero, scaleInDuration).setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
                        {
                        });
                        Invoke("SpawnEffect", delaySpawnEffect);

                        LeanTween.value(gameObject, spriteColor.a, 0, scaleInDuration).setOnUpdate((float alphaValue) =>
                        {
                            spriteColor.a = alphaValue;
                            spriteRenderer.color = spriteColor;
                        }).setEase(LeanTweenType.easeInOutQuad);
                    });
    }

    private void SpawnEffect()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }
}
