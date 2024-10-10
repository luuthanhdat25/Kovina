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
    private TMP_Text levelNumber;

    public void SetLevelNumber(int number) => levelNumber.text = number.ToString();

    public void AddButtonAction(UnityAction action) => button.onClick.AddListener(action);

    public void ResetRectTransformPosition(Quaternion parentRotation)
    {
        canvasRectTransform.SetLocalPositionAndRotation(Vector3.zero, parentRotation);
    }
}
