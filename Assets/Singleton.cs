using UnityEngine;

/// <summary>
/// Generic Singleton class used for creating a single instance of a MonoBehaviour-derived class
/// This class will destoy on load
/// </summary>
/// <typeparam name="T">The type of the MonoBehaviour-derived class</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    /// <summary>
    /// Get the single instance of the class
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            return SetUpInstance();
        }
    }

    private static T SetUpInstance()
    {
        instance = (T)FindObjectOfType(typeof(T));
        if (instance != null) return instance;
            
        GameObject gameObject = new GameObject();
        gameObject.name = typeof(T).Name;
        instance = gameObject.AddComponent<T>();
        return instance;
    }
}
