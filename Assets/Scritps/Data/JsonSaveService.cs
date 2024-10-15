using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public static class JsonSaveService
{
    public static bool SaveData<T>(string relativePath, T data)
    {
        string dataPath = Application.persistentDataPath + relativePath;
        string json = JsonConvert.SerializeObject(data);

        try
        {
            if (File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, json);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(dataPath))
                {
                    writer.Write(json);
                }
            }

            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
#endif
            return false; 
        }
    }

    public static T LoadData<T>(string relativePath)
    {
        string path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Can't load file at {path}. File doesn't exist");
#endif
            return default;
        }

        try
        {
            string json = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Fail to load data: {e.Message} {e.StackTrace}");
#endif
            return default;
        }
    }
}

