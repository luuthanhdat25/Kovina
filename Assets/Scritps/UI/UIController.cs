using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContronller : MonoBehaviour
{

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void SetTriggerAnimation()
    {
        _animator.SetTrigger("IsMenuSet");
    }
    public void ActionMenuHandler(GameObject playerActionOption)
    {
        if (playerActionOption == GameObject.Find("PlayButton"))
        {
            Debug.Log("isPlay");
        }
        else if (playerActionOption == GameObject.Find("CreditButton"))
        {
            Debug.Log("is Credit");
        }
        else if (playerActionOption == GameObject.Find("ExitButton")) 
        {
            Debug.Log("is Exit");
        }
    }
}
