using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigator : MonoBehaviour
{
    public GameObject bar;
    public int timeProcess;

    public GameObject currentScene;
    private GameObject intoScene;
    private UIEvenHandler controller = new UIEvenHandler();

    [SerializeField]
    private UIContronller _uIController;

    private void Start()
    {
        animateBar();
        intoScene = GameObject.Find("MenuPage");
        intoScene.SetActive(false);
    }
    public void animateBar()
    {
        LeanTween.scaleX(bar, 10.04f, timeProcess).setOnComplete(PlayMenu);
    }
    public void PlayMenu()
    {
        controller.ChangUIApearence(currentScene,intoScene);
        _uIController.SetTriggerAnimation();
    }

}
