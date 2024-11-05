using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDropdownHandler : MonoBehaviour
{
    public string[] keys;
    private TMP_Dropdown dropdown;
    private bool isUpdating = false; 

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown == null)
        {
            Debug.LogError("TMP_Dropdown component not found!");
        }
    }

    void Start()
    {
        LocalizationManager.Instance.OnLanguageChanged += UpdateOptions;
        UpdateOptions();
    }

    public void UpdateOptions()
    {
        if (isUpdating) return;
        isUpdating = true;

        if (dropdown != null && keys != null && keys.Length > 0)
        {
            // Store the current selection index
            int currentIndex = dropdown.value;

            dropdown.ClearOptions();
            var localizedOptions = new List<string>();

            foreach (var key in keys)
            {
                string localizedText = LocalizationManager.Instance.GetLocalizedText(key);
                localizedOptions.Add(localizedText);
            }

            dropdown.AddOptions(localizedOptions);
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Equals(LocalizationManager.Instance.GetCurrentLanguage() + "_Display"))
                {
                    dropdown.value = i;
                    break;
                }
            }
            dropdown.RefreshShownValue();
        }

        isUpdating = false; 
    }

    void OnDestroy()
    {
        LocalizationManager.Instance.OnLanguageChanged -= UpdateOptions; 
    }
}