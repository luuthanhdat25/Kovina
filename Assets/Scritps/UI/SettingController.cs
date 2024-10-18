using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // If you're using AudioMixer for volume control

public class SettingController : MonoBehaviour
{
    public AudioMixer audioMixer; 

    public void ChangeVolume(float volume)
    {
        Debug.Log($"Volume set to: {volume}");
        
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20); // Convert linear to decibel
    }

    public void ChangeLanguage(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("Changed to Vietnamese");
                LocalizationManager.Instance.SetLanguage("English");
                break;
            case 1:
                Debug.Log("Changed to Korean");
                LocalizationManager.Instance.SetLanguage("Vietnamese");
                break;
            case 2:
                Debug.Log("Changed to English");
                LocalizationManager.Instance.SetLanguage("Korean");
                break;
            default:
                Debug.LogWarning("Language index not recognized.");
                return; 
        }
    }
}