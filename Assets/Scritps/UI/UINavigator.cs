using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        AnimateBar();
        intoScene = GameObject.Find("MenuPage");
        intoScene.SetActive(false);
    }
    public void AnimateBar()
    {
        LeanTween.scaleX(bar, 10f, timeProcess).setOnComplete(PlayMenu);
    }
    public void PlayMenu()
    {
        StartCoroutine(controller.ChangUIApearence(currentScene, intoScene));
        _uIController.SetTriggerAnimation();
    }

}
