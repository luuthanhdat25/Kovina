using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem cloudParticle;

    private const float SIMULATE_TIME_BEFORE_PLAY = 15F;

    void Start()
    {
        cloudParticle.Simulate(SIMULATE_TIME_BEFORE_PLAY, true, true);
        cloudParticle.Play();
    }

    public void SetScale(Vector3 scale)
    {
        var shap = cloudParticle.shape;
        shap.scale = scale;
    }

    public void SetPosition(Vector3 position) => transform.localPosition = position;
}
