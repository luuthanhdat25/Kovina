using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    public void ChangeVolume(float volume)
    {
        Debug.Log(volume);
    }
    public void ChangeLangue(int index)
    {
        switch (index)
        {
            case 0: Debug.Log("Changed to VietNamese");break;
            case 1: Debug.Log("Changed to Korea"); break;
            case 2: Debug.Log("Changed to English"); break;
        }
    }

}
