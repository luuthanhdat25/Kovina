using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textCooldown;
    [SerializeField] private float timeCountDown = 180;

    private float remainTime;
    private bool isPaused = false; 
   
    void Start()
    {
        remainTime = timeCountDown;
        StartCoroutine(StartCountdown());
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
        GameManager.Instance.GameOver();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        textCooldown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
