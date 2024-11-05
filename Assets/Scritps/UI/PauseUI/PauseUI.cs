using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : Singleton<MonoBehaviour>
{
    [SerializeField]
    private RectTransform pauseMenu;
    public RectTransform PauseMenu => pauseMenu;

    [SerializeField]
    private Button continuteButton;
    public Button ContinuteButton => continuteButton;

    [SerializeField]
    private Button backToRoadMap;
    public Button BackToRoadMap => backToRoadMap;

    [SerializeField]
    private RectTransform blackBackground;
    public RectTransform BlackBackground => blackBackground;


    void Start()
    {
        pauseMenu.gameObject.SetActive(false);
        continuteButton.onClick.AddListener(TogglePauseGame);
        backToRoadMap.onClick.AddListener(() => LoadScene.Instance.LoadRoadmap());
    }

    public void TogglePauseGame()
    {
        GameManager.Instance.TogglePauseGame();
    }
}
