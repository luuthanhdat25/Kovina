using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCotnroller : MonoBehaviour
{
    private UIEvenHandler controller = new UIEvenHandler();

    public void PlayToSettingUI(GameObject menu, GameObject setting)
    {
        StartCoroutine(controller.ChangUIApearence(menu, setting));
    }
}
