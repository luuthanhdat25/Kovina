using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : PersistentSingleton<LoadScene>
{
    [SerializeField]
    private string gameScene = "Grid";

    [SerializeField]
    private string roadMapScene = "Roadmap";

    private int level;
    public int Level => level;

    public void LoadLevel(int level)
    {
        this.level = level;
        SceneManager.LoadScene(gameScene);
    }

    public void LoadRoadmap()
    {
        SceneManager.LoadScene(roadMapScene);
    }
}
