using System.Collections.Generic;
using UnityEngine;

public class ButtonLevelGenerator : MonoBehaviour
{
    private readonly string LEVELDATA_PATH = "/level-data.json";
    private LevelRepository levelRepository;

    [SerializeField]
    private ButtonLevel buttonLevelPrefab;

    [SerializeField]
    private List<Transform> levelNodeList;

    private void Start()
    {
        levelRepository = new LevelRepository(LEVELDATA_PATH);
        if (levelRepository.GetLevelList().Count == 0)
        {
            Level level1 = new Level()
            {
                NumberLevel = 1,
                NumberStar = 3
            };
            Level level2 = new Level()
            {
                NumberLevel = 2,
                NumberStar = 2
            };
            Level level3 = new Level()
            {
                NumberLevel = 3,
                NumberStar = 1
            };
            levelRepository.SaveLevel(level1);
            levelRepository.SaveLevel(level2);
            levelRepository.SaveLevel(level3);
        }

        for(int i = 0; i < levelNodeList.Count; i++)
        {
            ButtonLevel newButton = Instantiate(buttonLevelPrefab, levelNodeList[i]);
            int levelNumber = i + 1;
            newButton.name = "[Button] Level " + levelNumber;
            newButton.SetLevelNumber(levelNumber);
            newButton.ResetRectTransformPosition(levelNodeList[i].transform.localRotation);
            
            Level level = levelRepository.GetLevelByLevelNumber(levelNumber);
            newButton.ShowStar(level != null? level.NumberStar : 0);

            newButton.AddButtonAction(() =>
            {
                Debug.Log("Play Level: " + levelNumber);
                LoadScene.Instance.LoadLevel(levelNumber);
            });
        }
    }
}
