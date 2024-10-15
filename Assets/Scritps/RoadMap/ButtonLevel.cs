using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonLevel : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvasRectTransform;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Sprite starActiveSprite;

    [SerializeField]
    private Sprite starUnactiveSprite;

    [SerializeField] 
    private Image[] starImages;

    [SerializeField]
    private TMP_Text levelNumber;

    public void SetLevelNumber(int number) => levelNumber.text = number.ToString();

    public void AddButtonAction(UnityAction action) => button.onClick.AddListener(action);

    public void ResetRectTransformPosition(Quaternion parentRotation)
    {
        canvasRectTransform.SetLocalPositionAndRotation(Vector3.zero, parentRotation);
    }

    public void ShowStar(int numberOfStar)
    {
        if (numberOfStar < 0 || numberOfStar > 3) return;
        if(numberOfStar == 0)
        {
            foreach (var star in starImages)
            {
                star.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var star in starImages)
            {
                star.gameObject.SetActive(true);
            }

            foreach (var star in starImages)
            {
                if (numberOfStar > 0)
                {
                    star.sprite = starActiveSprite;
                    numberOfStar--;
                }
                else
                {
                    star.sprite = starUnactiveSprite;
                }
            }
        }
    }
}
