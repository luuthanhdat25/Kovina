using UnityEngine;

public class ExplosiveBox : MonoBehaviour, IObject
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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float DoAction()
    {
        return 0;
    }

    private float Explode()
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
        return scaleOutDuration + scaleInDuration;
    }

    private void SpawnEffect()
    {
        ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }

    public float DoAction(Cell cellPlaced)
    {
        cellPlaced.ClearContainObject();
        return Explode();
    }
}
