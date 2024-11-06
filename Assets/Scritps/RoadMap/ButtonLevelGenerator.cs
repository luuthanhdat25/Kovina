using System.Collections.Generic;
using UnityEngine;

public class ButtonLevelGenerator : MonoBehaviour
{
    [SerializeField]
    private ButtonLevel buttonLevelPrefab;

    [SerializeField]
    private List<Transform> levelNodeList;

    private void Start()
    {
        var levelRepository = LoadScene.Instance.LevelRepository;
        for (int i = 0; i < levelNodeList.Count; i++)
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
