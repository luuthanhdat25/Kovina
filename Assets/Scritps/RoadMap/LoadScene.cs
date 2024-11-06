using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : PersistentSingleton<LoadScene>
{
    [SerializeField]
    private string gameScene = "Grid";

    [SerializeField]
    private string roadMapScene = "Roadmap";

    private int levelNumber;
    public int LevelNumber => levelNumber;

    private LevelSetUpSO levelSetUpSO;
    private LevelRepository levelRepository;
    public LevelRepository LevelRepository => levelRepository;
    public LevelSetUpSO LevelSetUpSO => levelSetUpSO;

    private readonly string RESOURCE_LEVELDATA_PATH = "Level/Level_";
    private readonly string PERSISTANCE_LEVELDATA_PATH = "/level-data.json";

    public void LoadLevel(int levelNumber)
    {
        this.levelNumber = levelNumber;
        SceneManager.LoadScene(gameScene);
    }

    protected override void Awake()
    {
        base.Awake();
        if(levelRepository == null)
        {
            levelRepository = new LevelRepository(PERSISTANCE_LEVELDATA_PATH);
        }

        this.levelSetUpSO = LoadLevelSetUpSO(levelNumber);
    }

    private LevelSetUpSO LoadLevelSetUpSO(int level)
    {
        int levelNumber = level <= 0 ? 1 : level;

        LevelSetUpSO levelGridData = Resources.Load<LevelSetUpSO>(RESOURCE_LEVELDATA_PATH + levelNumber);

        if (levelGridData != null)
        {
            if (levelGridData.ItemTypes.Length < 2)
            {
                Debug.LogError("[ItemTraditionalSpawner] Number of ItemType can't less than 2");
            }
            return levelGridData;
        }
        else
        {
            Debug.LogError("[ItemTraditionalSpawner] " + "File" + RESOURCE_LEVELDATA_PATH + levelNumber + " doesn't exist!");
            return null;
        }
    }

    public void LoadRoadmap()
    {
        SceneManager.LoadScene(roadMapScene);
    }
}
