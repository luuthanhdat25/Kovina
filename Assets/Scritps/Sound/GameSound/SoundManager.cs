using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] private SoundSo menuBackgroundSound;
    [SerializeField] private SoundSo clickSound;
    [SerializeField] private SoundSo mergedSound;
    [SerializeField] private SoundSo putDownSound;
    [SerializeField] private SoundSo victorySound;
    [SerializeField] private SoundSo roadMapBackGroundSound;
    [SerializeField] private SoundSo playingBackGroundSound;

    private Button[] buttons;
    void Start()
    {
        SoundPooling.Instance.StopAll();
        buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayClickButton());
        }

        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        PlayBackgroundSound(currentScene.name);

    }   

    private void PlayBackgroundSound(string sceneName)
    {
        if (sceneName == SceneEnum.Roadmap.ToString())
        {
            PlayRoadMapBackgroundSound();
        }
        else if (sceneName == SceneEnum.Grid.ToString())
        {
            PlayPlayingBackgroundSound();
        }
        else if (sceneName == SceneEnum.Menu.ToString())
        {
            PlayMenuBackgroundSound();
        }
    }
    public void PlayClickButton()
    {
        Debug.Log("Play Click Sound");
        SoundPooling.instance.CreateSound(clickSound,0,0);
    }

    public void PlayMergeSound()
    {
        Debug.Log("play merge sound");
        SoundPooling.Instance.CreateSound (mergedSound, 0,0);
    }

    public void PlayPutDownSound()
    {
        Debug.Log("play put down sound");
        SoundPooling.Instance.CreateSound(putDownSound, 0,0);
    }

    public void PlayVictorySound()
    {
        Debug.Log("play put down sound");
        SoundPooling.Instance.CreateSound (victorySound, 0,0);
    }

    public void PlayRoadMapBackgroundSound()
    {
        Debug.Log("Play Road Map Background Sound");
        SoundPooling.Instance.CreateSound(roadMapBackGroundSound, 0, 0);
    }
    public void PlayMenuBackgroundSound()
    {
        Debug.Log("Play Menu background Sound");
        SoundPooling.Instance.CreateSound(menuBackgroundSound,0,0);
    }
    public void PlayPlayingBackgroundSound()
    {
        Debug.Log("Play Playing Background Sound");
        SoundPooling.Instance.CreateSound(playingBackGroundSound, 0, 0);
    }
}
