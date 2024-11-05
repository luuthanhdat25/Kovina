using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPooling : Singleton<SoundPooling>
{
    private List<SoundEmitter> soundEmitterPool = new();

    [SerializeField]
    private SoundEmitter soundEmitterPrefab;


    [SerializeField]
    private int maxPoolSize = 100;

    public void CreateSound(SoundSo soundSO, float minRandomPitch, float maxRandomPitch)
    {
        if (!CanPlaySound(soundSO)) return;
        SoundEmitter soundEmitter = GetFromPool(soundSO);
        soundEmitter.gameObject.SetActive(true);
        soundEmitter.transform.position = Camera.main.transform.position;
        soundEmitter.SetRandomPitch(minRandomPitch, maxRandomPitch);

        soundEmitter.Play();
    }
    public void StoppAllSound()
    {
        foreach (var soundEmmit in soundEmitterPool)
        {
            if (!soundEmmit.gameObject.activeSelf)
            {
                soundEmmit.Stop();
            }
        }
    }
    public bool CanPlaySound(SoundSo soundSO)
    {
        return soundEmitterPool.Count < maxPoolSize;
    }

    private SoundEmitter GetFromPool(SoundSo soundSO)
    {
        foreach (var soundEmmit in soundEmitterPool)
        {
            if (!soundEmmit.gameObject.activeSelf && soundEmmit.SoundSO == soundSO)
            {
                return soundEmmit;
            }
        }
        SoundEmitter newSoundEmitter = Instantiate(soundEmitterPrefab).Initialize(soundSO);
        newSoundEmitter.transform.parent = transform;
        newSoundEmitter.transform.name = soundSO.name;
        soundEmitterPool.Add(newSoundEmitter);
        return newSoundEmitter;
    }

    public void Despawn(SoundEmitter soundEmitter)
    {
        soundEmitter.gameObject.SetActive(false);

        if (soundEmitterPool.Count >= maxPoolSize && soundEmitterPool.Contains(soundEmitter))
        {
            soundEmitterPool.Remove(soundEmitter);
        }
    }

    public void StopAll()
    {
        soundEmitterPool.ForEach(soundEmitter => soundEmitter.Stop());
    }
}
