using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] private SoundSO menuBackgroundSound;
    [SerializeField] private SoundSO clickSound;
    [SerializeField] private SoundSO mergedSound;
    [SerializeField] private SoundSO putDownSound;
    [SerializeField] private SoundSO victorySound;
    [SerializeField] private SoundSO lostSound;
    [SerializeField] private SoundSO roadMapBackGroundSound;
    [SerializeField] private SoundSO playingBackGroundSound;
    [SerializeField] private SoundSO explosiveBoxSound;
    [SerializeField] private SoundSO quesitonBoxSound;

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


    public void PlayLostSound()
    {
        Debug.Log("Play Lost Sound");
        SoundPooling.Instance.CreateSound(lostSound,0,0);
    }

    public void PlayClickButton()
    {
        Debug.Log("Play Click Sound");
        SoundPooling.instance.CreateSound(clickSound,0,0);
    }

    public void PlayMergeSound()
    {
        Debug.Log("play merge sound");
        SoundPooling.Instance.CreateSound (mergedSound, -0.1f, 0.1f);
    }

    public void PlayPutDownSound()
    {
        Debug.Log("play put down sound");
        SoundPooling.Instance.CreateSound(putDownSound, -0.1f, 0.1f);
    }

    public void PlayVictorySound()
    {
        Debug.Log("play Victory sound");
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

    public void PlayExplosiveBoxSound()
    {
        SoundPooling.Instance.CreateSound(explosiveBoxSound, -0.1f, 0.1f);
    }

    public void PlayQuestionBoxSound()
    {
        SoundPooling.Instance.CreateSound(quesitonBoxSound, -0.1f, 0.1f);
    }
}
