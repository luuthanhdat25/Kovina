using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Cooldown : Singleton<Cooldown>
{
    [SerializeField] private TMP_Text textCooldown;
    [SerializeField] private float totalTime = 30;


    private float remainTime;
    private bool isPaused = false; 
   
    void Start()
    {
        remainTime = totalTime;
        StartCoroutine(StartCountdown());
        UISingleton.Instance.IsPauseEvent += OnPauseHandler_Event;
    }

    private void OnPauseHandler_Event(object sender, UISingleton.IsPauseEvent_Args e)
    {
        isPaused = e.isPause;
    }
    
    IEnumerator StartCountdown()
    {
        while (remainTime > 0)
        {
            if (!isPaused)
            {
                UpdateTimerText();
                remainTime -= 1f;
            }
            yield return new WaitForSeconds(1f);
        }

        remainTime = 0;
        UpdateTimerText();
        Debug.Log("Countdown finished!");
        UISingleton.Instance.CallOnEndGame(2);
    }

    public void FreezeTime()
    {
        isPaused = !isPaused;
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        textCooldown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
