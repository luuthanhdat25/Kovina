using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "New SoundSO", menuName = "ScriptableObjects/SoundSO", order = 5)]
public class SoundSo : ScriptableObject
{
    public AudioClip AudioClip;
    public AudioMixerGroup AudioMixerGroup;
    public float Volume = 1;
    public bool Loop;
    public bool PlayOnAwake = true;
    public bool FrequentSound;

}
