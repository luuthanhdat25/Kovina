using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleCancelSetting : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject menuPanel;

    private UIEvenHandler controller = new UIEvenHandler();

    public void OnPointerDownEven()
    {
        Debug.Log("there is exit setting");
        if (settingPanel.activeSelf)
        {
            StartCoroutine(controller.ChangUIApearence(settingPanel, menuPanel));
        }
    }
}