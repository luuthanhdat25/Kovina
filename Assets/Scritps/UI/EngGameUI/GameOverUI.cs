using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject[] star;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] GameObject Victory;
    [SerializeField] GameObject Lost;
    [SerializeField] GameObject ButtonContainer;
    [SerializeField] Button backToRoadMap;
    [SerializeField] Button nextLevel;
    [SerializeField] Button replayButton;
    [SerializeField] RectTransform backBackground;
    private Vector2 startSize;

    void Start()
    {
        backToRoadMap.onClick.AddListener(() => { SceneManager.LoadScene(SceneEnum.Roadmap.ToString()); });
        endGameUI.SetActive(false);
        startSize = endGameUI.transform.localScale;

        int numberOfLevel = LoadScene.Instance.LevelNumber;
        if (numberOfLevel == 16)
        {
            nextLevel.gameObject.SetActive(false);
        }
        else
        {
            nextLevel.onClick.AddListener(() => LoadScene.Instance.LoadLevel(numberOfLevel + 1));
        }
    }

    public void InitializeEndGameUI(int starNo)
    {
        endGameUI.gameObject.SetActive(true);
        backBackground.gameObject.SetActive(true);
        Debug.Log($"star number {starNo}");
        endGameUI.transform.localScale = Vector3.zero;
        ButtonContainer.transform.localScale = Vector3.zero;
        Victory.SetActive(false);
        Lost.SetActive(false);
        
        PlayEndGameUI(starNo);
        if (starNo > 0)
        {
            SoundManager.Instance.PlayVictorySound();
            Victory.SetActive(true); 
        }
        else
        {
            SoundManager.Instance.PlayLostSound();
            Lost.SetActive(true);
        }

        SaveResult(starNo);
    }

    private void SaveResult(int starNo)
    {
        if (starNo <= 0) return;

        LevelRepository levelRepository = LoadScene.Instance.LevelRepository;
        Level level = levelRepository.GetLevelByLevelNumber(starNo);
        
        if(level == null)
        {
            Level newLevel = new Level
            {
                NumberLevel = LoadScene.Instance.LevelNumber,
                NumberStar = starNo
            };
            levelRepository.SaveLevel(newLevel);
        }
        else
        {
            if(starNo > level.NumberStar)
            {
                level.NumberStar = starNo;
                levelRepository.SaveLevel(level);
            }
        }
    }

    private void ShowStar(int starNo)
    {
        var sequence = LeanTween.sequence();
        for (int i = 0; i < starNo; i++)
        {
            int index = i;
            sequence.append(() => PlayStar(star[index]));
            sequence.append(0.6f);
        }
        sequence.append(() => PlayButton());
    }

    private void PlayStar(GameObject star)
    {
        LeanTween.scale(star, new Vector3(1, 1, 1), 0.5f);
        LeanTween.rotate(star, new Vector3(0, 0, 720), 0.5f);
    }

    private void PlayButton()
    {
        LeanTween.scale(ButtonContainer, new Vector3(1.75f, 1.75f, 1.75f), 0.5f);
    }

    private void PlayEndGameUI(int starNo)
    {
        LeanTween.scale(endGameUI, startSize, 0.5f)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(0.2f, () => ShowStar(starNo));
            });
    }
}
