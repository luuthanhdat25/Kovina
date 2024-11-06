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
        }
    }

    void LoadLocalization()
    {
        LoadCurrentLanguage();

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

    private void LoadCurrentLanguage()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, "GameText.json");

        if (File.Exists(fullPath))
        {
            string fileContent = File.ReadAllText(fullPath);
            CurrentLanguage savedLanguage = JsonUtility.FromJson<CurrentLanguage>(fileContent);
            currentLanguage = savedLanguage.currentLanguage;
        }
        else
        {
            currentLanguage = "English";
            SaveCurrentLanguage();
        }

        Debug.Log("Current language loaded: " + currentLanguage);
    }

    private void SaveCurrentLanguage()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, "GameText.json");
        CurrentLanguage languageData = new CurrentLanguage { currentLanguage = currentLanguage };
        string json = JsonUtility.ToJson(languageData, true);
        File.WriteAllText(fullPath, json);
    }

    private void ParseLocalizationFile(string jsonString)
    {
        var loadedData = JsonUtility.FromJson<LocalizationData>(jsonString);
        localizedText = new Dictionary<string, Dictionary<string, string>>();

        AddTextToDictionary(loadedData.MenuText);
        AddTextPauseLangue(loadedData.PauseData);
        AddTextCreditLangue(loadedData.Credit);
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

    private void AddTextCreditLangue(Credit credit)
    {
        foreach (var property in typeof(Credit).GetFields())
        {
            var key = property.Name;
            var creditText = (TextData)property.GetValue(credit);
            localizedText[key] = new Dictionary<string, string>
            {
                { "English", creditText.English },
                { "Vietnamese", creditText.Vietnamese },
                { "Korean", creditText.Korean }
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
                { "Vietnamese", settingLanguage.Display.Vietnamese }
            };
            localizedText[key + "_Display"] = displayText;
        }
    }

    public string GetLocalizedText(string key)
    {
        if (localizedText.TryGetValue(key, out var languageTexts))
        {
            if (languageTexts.TryGetValue(currentLanguage, out var text))
            {
                return text;
            }
        }

        Debug.LogWarning($"Localized text for key '{key}' in language '{currentLanguage}' not found.");
        return "Localized text not found";
    }

    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public void SetLanguage(string language)
    {
        currentLanguage = language;
        SaveCurrentLanguage();
        OnLanguageChanged?.Invoke();
    }
}

[System.Serializable]
public class CurrentLanguage
{
    public string currentLanguage;
}

[System.Serializable]
public class LocalizationData
{
    public LanguageText MenuText;
    public SettingData SettingText;
    public PauseData PauseData;
    public Credit Credit;
}

[System.Serializable]
public class Credit
{
    public TextData Developer;
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
