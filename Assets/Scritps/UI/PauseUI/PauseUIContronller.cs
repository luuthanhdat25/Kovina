using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUIContronller : Singleton<MonoBehaviour>
{
    [SerializeField] PauseItem pauseItem;
    [SerializeField] Button pauseButton;
    [SerializeField] Button countinueButton;
    [SerializeField] Button backToRoadMap;
    [SerializeField] EndGame endGameUI;

    [SerializeField] HandleEvenUI handleEvenUI;


    void Start()
    {
        pauseItem.gameObject.SetActive(false);
        pauseButton.onClick.AddListener(OpenPauseUI);
        backToRoadMap.onClick.AddListener(BackToRoadMap);
        endGameUI.gameObject.SetActive(false);
        handleEvenUI.OnCancelPause += ClosePauseUI;
        handleEvenUI.gameObject.SetActive(false);

        UISingleton.Instance.IsEndGame += OnEndGameEvent;
    }

    private void OnEndGameEvent(object sender, UISingleton.IsEndGame_Args e)
    {
        PlayEndGame(e.star);
    }
    private void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void BackToRoadMap()
    {
        SceneManager.LoadScene("Roadmap");
    }

    public void PlayEndGame(int star)
    {
        Debug.Log("endgamed");
        endGameUI.gameObject.SetActive(true);
        endGameUI.SetStarNo(star);
        endGameUI.InitializeEndGameUI();
    }
    private void OpenPauseUI()
    {
        handleEvenUI.gameObject.SetActive(true);
        pauseItem.gameObject.SetActive(true);
        pauseButton.interactable = false;
        UISingleton.Instance.CallOnPauseChanges(true);
    }

    private void ClosePauseUI()
    {
        pauseItem.gameObject.SetActive(false);
        handleEvenUI.gameObject.SetActive(false);
        pauseButton.interactable = true;
        UISingleton.Instance.CallOnPauseChanges(false);

    }
}
