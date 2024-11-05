using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager>
{
    public static LocalizationManager Instance;

    private Dictionary<string, Dictionary<string, string>> localizedText;
    private string currentLanguage;
    public event Action OnLanguageChanged;

    void Awake()
    {
        InitializeSingleton();
        LoadLocalization();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }
    }

    void LoadLocalization()
    {
        TextAsset localizationFile = Resources.Load<TextAsset>("Localization/GameText");

        if (localizationFile != null)
        {
            ParseLocalizationFile(localizationFile.text);
            Debug.Log("Loaded text: " + string.Join(", ", localizedText.Keys));
        }
        else
        {
            Debug.LogError("Localization file not found!");
        }
    }

    private void ParseLocalizationFile(string jsonString)
    {
        var loadedData = JsonUtility.FromJson<LocalizationData>(jsonString);
        currentLanguage = loadedData.CurrentLanguage;

        localizedText = new Dictionary<string, Dictionary<string, string>>();
        var x = loadedData.PauseData;
        AddTextToDictionary(loadedData.MenuText);
        AddTextPauseLangue(loadedData.PauseData);
        AddSettingTextToDictionary(loadedData.SettingText);
    }
    private void AddTextPauseLangue(PauseData pauseData)
    {
        foreach (var property in typeof(PauseData).GetFields())
        {
            var key = property.Name;
            var pauseDatatext = (TextData)property.GetValue(pauseData);


            localizedText[key] = new Dictionary<string, string>
            {
                { "English", pauseDatatext.English },
                { "Vietnamese", pauseDatatext.Vietnamese },
                { "Korean", pauseDatatext.Korean }
            };
        }
    }
    private void AddTextToDictionary(LanguageText languageText)
    {
        foreach (var property in typeof(LanguageText).GetFields())
        {
            var key = property.Name;
            var textData = (TextData)property.GetValue(languageText);

            localizedText[key] = new Dictionary<string, string>
            {
                { "English", textData.English },
                { "Vietnamese", textData.Vietnamese },
                { "Korean", textData.Korean }
            };
        }
    }
    private void AddSettingTextToDictionary(SettingData settingData)
    {
        foreach (var property in typeof(SettingData).GetFields())
        {
            var key = property.Name;
            var settingLanguage = (SettingLanguage)property.GetValue(settingData);

            var displayText = new Dictionary<string, string>
            {
                { "English", settingLanguage.Display.English },
                { "Vietnamese", settingLanguage.Display.Vietnamese },
                { "Korean", settingLanguage.Display.Korean }
            };

            localizedText[key + "_Display"] = displayText;
        }
    }

    public string GetLocalizedText(string key)
    {
        if (!localizedText.ContainsKey(key))
        {
            Debug.LogWarning($"Key '{key}' does NOT exist in localizedText.");
            return "Localized text not found";
        }

        if (localizedText[key].ContainsKey(currentLanguage))
        {
            Debug.Log($"Key '{key}' found for language '{currentLanguage}'.");
            return localizedText[key][currentLanguage];
        }

        if (localizedText[key].ContainsKey(currentLanguage + "_Display"))
        {
            Debug.Log($"Key '{key}' found for language '{currentLanguage}_Display'.");
            return localizedText[key][currentLanguage + "_Display"];
        }

        Debug.LogWarning($"No text found for key '{key}' and language '{currentLanguage}'.");
        return "Localized text not found";
    }

    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public void SetLanguage(string language)
    {
        currentLanguage = language;
        UpdateCurrentLanguageInJson();
        Debug.Log("Current language set to: " + currentLanguage);
        OnLanguageChanged?.Invoke();
    }

    private void UpdateCurrentLanguageInJson()
    {
        TextAsset localizationFile = Resources.Load<TextAsset>("Localization/GameText");

        if (localizationFile == null)
        {
            Debug.LogError("Localization file not found!");
            return;
        }

        var localizationData = JsonUtility.FromJson<LocalizationData>(localizationFile.text);
        localizationData.CurrentLanguage = currentLanguage;

        string updatedJson = JsonUtility.ToJson(localizationData, true);
        File.WriteAllText(Path.Combine(Application.dataPath, "Resources/Localization/GameText.json"), updatedJson);
    }
}

// Data classes
[System.Serializable]
public class LocalizationData
{
    public LanguageText MenuText;
    public SettingData SettingText;
    public PauseData PauseData;
    public string CurrentLanguage;
}
[System.Serializable]
public class PauseData
{
    public TextData Continue;
    public TextData BackToMenu;
    public TextData BackToRoadMap;
}

[System.Serializable]
public class LanguageText
{
    public TextData Play;
    public TextData Credit;
    public TextData Exit;
}

[System.Serializable]
public class SettingData
{
    public SettingLanguage Vietnamese;
    public SettingLanguage English;
    public SettingLanguage Korean;
}

[System.Serializable]
public class SettingLanguage
{
    public TextData Display;
}

[System.Serializable]
public class TextData
{
    public string English;
    public string Vietnamese;
    public string Korean;
}
