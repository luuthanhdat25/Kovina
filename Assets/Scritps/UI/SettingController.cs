using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // If you're using AudioMixer for volume control

public class SettingController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider audioSlider;

    private void Start()
    {
        float backgroundVolume;
        audioMixer.GetFloat("Background", out backgroundVolume);
        audioSlider.value = Mathf.Pow(10, backgroundVolume / 20); ;

    }
    public void ChangeVolume(float volume)
    {
        Debug.Log($"Volume set to: {volume}");
        
        audioMixer.SetFloat("Background", Mathf.Log10(volume) * 20); 
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