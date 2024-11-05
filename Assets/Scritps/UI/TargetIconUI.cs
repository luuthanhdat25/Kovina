using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetIconUI : MonoBehaviour
{
    [SerializeField]
    private Image targetIconImage;

    [SerializeField]
    private TextMeshProUGUI targetText;

    public ItemType ItemType { get; set; }

    public void SetTargetIconImage(Sprite sprite) => targetIconImage.sprite = sprite;

    public void UpdateProcessText(int updatedValue) => targetText.text = updatedValue.ToString();
}
