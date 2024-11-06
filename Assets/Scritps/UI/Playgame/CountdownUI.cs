using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textCooldown;

    private float remainTime;

    void Start()
    {
        remainTime = LoadScene.Instance.LevelSetUpSO.TimePlay;
        UpdateTimerText(); // Initialize with starting time
        StartCoroutine(StartCountdownAfterDelay(1f)); // Start countdown with a 1-second delay
    }

    private System.Collections.IEnumerator StartCountdownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (remainTime > 0)
        {
            if (!GameManager.Instance.IsGamePlaying()) yield return null;

            remainTime -= Time.deltaTime;
            if (remainTime <= 0)
            {
                remainTime = 0;
                Debug.Log("Countdown finished!");
                GameManager.Instance.GameOver();
            }
            UpdateTimerText();
            yield return null; // Wait until the next frame
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        textCooldown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
