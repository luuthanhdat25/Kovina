using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textCooldown;
    [SerializeField] private float timeCountDown = 180f;

    private float remainTime;
    private bool isPaused = false;

    void Start()
    {
        remainTime = timeCountDown;
        UpdateTimerText(); // Initialize with starting time
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (!isPaused && remainTime > 0)
        {
            remainTime -= Time.fixedDeltaTime;
            if (remainTime <= 0)
            {
                remainTime = 0;
                Debug.Log("Countdown finished!");
                GameManager.Instance.GameOver();
            }
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        textCooldown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
