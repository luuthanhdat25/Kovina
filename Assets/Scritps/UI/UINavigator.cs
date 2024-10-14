using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UINavigator : MonoBehaviour
{
    public GameObject bar;
    public int timeProcess;

    public GameObject currentUI;
    private GameObject intoUI;
    private GameObject settingUI;
    private UIEvenHandler controller = new UIEvenHandler();

    [SerializeField]
    private UIContronller _uIController;

    private void Start()
    {
        AnimateBar();
        intoUI = GameObject.Find("MenuPage");
        settingUI = GameObject.Find("SettingPage");
        intoUI.SetActive(false);
        settingUI.SetActive(false);
    }
    public void AnimateBar()
    {
        LeanTween.scaleX(bar, 10f, timeProcess).setOnComplete(PlayMenu);
    }
    public void PlayMenu()
    {
        StartCoroutine(controller.ChangUIApearence(currentUI, intoUI));
        _uIController.SetTriggerAnimation();
    }
  

}
