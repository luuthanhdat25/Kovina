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
        for(int i = 0; i < levelNodeList.Count; i++)
        {
            ButtonLevel newButton = Instantiate(buttonLevelPrefab, levelNodeList[i]);
            int levelNumber = i + 1;
            newButton.name = "[Button] Level " + levelNumber;
            newButton.SetLevelNumber(levelNumber);
            newButton.ResetRectTransformPosition(levelNodeList[i].transform.localRotation);
            newButton.AddButtonAction(() =>
            {
                Debug.Log("Play Level: " + levelNumber);
            });
        }
    }
}
