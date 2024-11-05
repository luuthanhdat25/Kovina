using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key; // Key to access localized text

    void Start()
    {
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    public void UpdateText()
    {
        TextMeshProUGUI textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            Debug.Log("Loaded component");
            textComponent.text = LocalizationManager.Instance.GetLocalizedText(key);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }
}