using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadLevel : PersistentSingleton<LoadLevel>
{
    [SerializeField]
    private string defaultScene = "Grid";

    private int level;
    public int Level => level;

    public void Load(int level)
    {
        this.level = level;
        SceneManager.LoadScene(defaultScene);
    }
}
