using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] SoundSo backgroundSound;
    [SerializeField] SoundSo clickSound;
    [SerializeField] SoundSo mergedSound;
    [SerializeField] SoundSo putDownSound;
    [SerializeField] SoundSo victorySound;


    private Button[] buttons;
    void Start()
    {
        SoundPooling.Instance.CreateSound(backgroundSound,0,0);
        buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayClickButton());
        }
    }

    public void PlayClickButton()
    {
        SoundPooling.instance.CreateSound(clickSound,0,0);
    }

    public void PlayMergeSound()
    {
        SoundPooling.Instance.CreateSound (mergedSound, 0,0);
    }

    public void PlayPutDownSound()
    {
        SoundPooling.Instance.CreateSound(putDownSound, 0,0);
    }

    public void PlayVictorySound()
    {
        SoundPooling.Instance.CreateSound (victorySound, 0,0);
    }
}
