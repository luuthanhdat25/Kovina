using System.Collections.Generic;
using System.Linq;

public class LevelRepository : Repository<LevelData>
{
    private LevelData levelData;

    public LevelRepository(string path) : base(path)
    {
        levelData = Get();

        // Instantiate file if don't have and data exist
        if (levelData == null || levelData.Levels == null)
        {
            levelData = new LevelData();
            levelData.Levels = new List<Level>();

            Save(levelData);
        }
    }

    public List<Level> GetLevelList() => levelData.Levels;

    public void SaveLevel(Level level)
    {
        Level existLevel = GetLevelByLevelNumber(level.NumberLevel);

        if (existLevel != null)
        {
            existLevel.NumberStar = level.NumberStar;
        }
        else
        {
            levelData.Levels.Add(level);
        }

        Save(levelData);
    }

    public Level GetLevelByLevelNumber(int levelNumber)
    {
        return (from level in levelData.Levels
                where level.NumberLevel == levelNumber
                select level).FirstOrDefault();
    }
}
