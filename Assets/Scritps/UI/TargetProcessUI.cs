using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TargetProcessUI : MonoBehaviour
{
    [SerializeField]
    private string levelLoadPath = "Level/Level_";

    [SerializeField]
    private TargetIconUI templateTargetIconUI;

    [SerializeField]
    private RectTransform templateParent;

    [SerializeField]
    private Slider processSlider;

    [SerializeField]
    private StarProcessUI[] starProcessUIs;

    [SerializeField]
    private RectTransform topSlider, botSlider;

    private List<TargetIconUI> targetIconUIs = new List<TargetIconUI>();
    private ItemSetUp[] itemSetUps;
    private int baseItemNumber;
    [SerializeField] private float[] starMilestones = { 1f / 3f, 2f / 3f, 1f };

    private void Start()
    {
        SetTargetProcessBar();
        PositionStars();
        TrayManager.Instance.OnCompletedMatchItem += TrayManager_OnCompletedMatchItem;
    }

    private void PositionStars()
    {
        RectTransform fillArea = processSlider.fillRect;
        float sliderHeight = fillArea.rect.height;

        float distance = sliderHeight / 3;
        Debug.Log($"Distance between each star: {distance}");

        Vector3 fillAreaPosition = fillArea.position;
        float yOffset = distance;

        for (int i = 0; i < starProcessUIs.Length; i++)
        {
            Vector3 starPosition = fillAreaPosition + Vector3.up * yOffset;
            yOffset += distance;
            RectTransform rectTransform = starProcessUIs[i].GetComponent<RectTransform>();
            rectTransform.position = starPosition;
        }
    }


    private void TrayManager_OnCompletedMatchItem(object sender, TrayManager.CompletedMatchItemEventArg e)
    {
        ItemSetUp item = itemSetUps.FirstOrDefault(i => i.ItemType == e.ItemType);
        int currentNumberItem = item.Number;
        if(currentNumberItem > 0)
        {
            currentNumberItem -= 3;
            UpdateProcess(e.ItemType, currentNumberItem);
        }
    }

    public int GetCurrentStars()
    {
        float currentValue = processSlider.value;
        int starCount = 0;

        for (int i = 0; i < starMilestones.Length; i++)
        {
            if (currentValue >= starMilestones[i])
            {
                starCount++;
            }
            else
            {
                break;
            }
        }

        return starCount;
    }


    private void SetTargetProcessBar()
    {
        itemSetUps = LoadItemSetup();
        baseItemNumber = itemSetUps.Sum(item => item.Number); 

        foreach (ItemSetUp itemSetup in itemSetUps)
        {
            TargetIconUI newIconUI = Instantiate(templateTargetIconUI, templateParent);
            newIconUI.ItemType = itemSetup.ItemType;
            newIconUI.name = "Challenge: " + itemSetup.ItemType;
            Sprite sprite = ItemTraditionalManager.Instance.Spawner.GetSpriteForItem(newIconUI.ItemType);
            newIconUI.SetTargetIconImage(sprite);
            newIconUI.UpdateProcessText(itemSetup.Number);
            targetIconUIs.Add(newIconUI);
        }
        templateTargetIconUI.gameObject.SetActive(false);
    }

    private ItemSetUp[] LoadItemSetup()
    {
        LoadScene loadLevel = LoadScene.Instance;
        int levelNumber;
        if (loadLevel != null)
        {
            levelNumber = loadLevel.Level <= 0 ? 1 : loadLevel.Level;
        }
        else
        {
            levelNumber = 1;
        }

        LevelSetUpSO levelGridData = Resources.Load<LevelSetUpSO>(levelLoadPath + levelNumber);

        if (levelGridData != null)
        {
            if (levelGridData.ItemTypes.Length < 2)
            {
                Debug.LogError("[ItemTraditionalSpawner] Number of ItemType can't less than 2");
            }
            return levelGridData.ItemSetUp;
        }
        else
        {
            Debug.LogWarning("[ItemTraditionalSpawner] " + "File" + levelLoadPath + levelNumber + " doesn't exist!");
            return null;
        }
    }

    public void UpdateProcess(ItemType itemType, int updatedValue)
    {
        TargetIconUI icon = targetIconUIs.FirstOrDefault(icon => icon.ItemType == itemType);
        if (icon != null) {
            icon.UpdateProcessText(updatedValue);

            for (int i = 0; i < itemSetUps.Length; i++)
            {
                if (itemSetUps[i].ItemType == itemType)
                {
                    itemSetUps[i].Number = updatedValue;
                    break;
                }
            }
            int currentNumberItem = itemSetUps.Sum(item => item.Number);
            float targetValue = 1 - (float)currentNumberItem / baseItemNumber;

            LeanTween.cancel(processSlider.gameObject);

            LeanTween.value(processSlider.gameObject, processSlider.value, targetValue, 0.5f)
                .setOnUpdate((float value) => {
                    processSlider.value = value;

                    if (value == 1)
                    {
                        starProcessUIs[2].ShowItem();
                        GameManager.Instance.GameOver();
                    }
                    else if (value > 2f / 3f)
                    {
                        starProcessUIs[1].ShowItem();
                    }
                    else if (value > 1f / 3f)
                    {
                        starProcessUIs[0].ShowItem();
                    }
                }).setEase(LeanTweenType.easeInSine);
        } 
    }
}
