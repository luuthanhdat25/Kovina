using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUIContronller : MonoBehaviour
{
    [SerializeField] PauseItem pauseItem;
    [SerializeField] Button pauseButton;
    [SerializeField] Button countinueButton;
    [SerializeField] Button backToRoadMap;


    [SerializeField] HandleEvenUI handleEvenUI; 
    void Start()
    {
        pauseItem.gameObject.SetActive(false);
        pauseButton.onClick.AddListener(OpenPauseUI);
        backToRoadMap.onClick.AddListener(BackToRoadMap);

        handleEvenUI.OnCancelPause += ClosePauseUI;
        handleEvenUI.gameObject.SetActive(false);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void BackToRoadMap()
    {
        SceneManager.LoadScene("Roadmap");
    }
    private void OpenPauseUI()
    {
        handleEvenUI.gameObject.SetActive(true);
        pauseItem.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        UISingleton.Instance.OnOpenPauseUI.Invoke();
    }

    private void ClosePauseUI()
    {
        pauseItem.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        handleEvenUI.gameObject.SetActive(false );
        UISingleton.Instance.OnClosePauseUI.Invoke();
    }
}
