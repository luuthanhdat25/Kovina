using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandleEvenUI : MonoBehaviour
{
    public Action OnCancelPause;
    public void OnPointerDownEven()
    {
        Debug.Log("is cancel pause ui");
        OnCancelPause.Invoke();
    }
}
