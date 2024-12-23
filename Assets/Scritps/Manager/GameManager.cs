using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event Action OnGamePaused;
    public event Action OnGameUnpaused;

    public class OnStateChangedEventArgs : EventArgs
    {
        public GameState NewGameState;
    }

    public enum GameState
    {
        GamePlaying,
        GameOver,
        GamePaused
    }

    private GameState state;

    protected void Awake()
    {
        state = GameState.GamePlaying;
        Time.timeScale = 1;
    }

    private void Start()
    {
        //SoundPooling.Instance.CreateSound(gameMusicBackgroundSO, 0, 0);
    }

    public void TogglePauseGame()
    {
        if (state == GameState.GameOver) return;

        if (state != GameState.GamePaused)
        {
            ChangeState(GameState.GamePaused);
            Time.timeScale = 0;
            OnGamePaused?.Invoke();
        }
        else
        {
            ChangeState(GameState.GamePlaying);
            Time.timeScale = 1;
            OnGameUnpaused?.Invoke();
        }

    }

    private void ChangeState(GameState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            NewGameState = newState
        });
    }

    public bool IsGamePlaying() => state == GameState.GamePlaying;

    public void GameOver()
    {
        SoundPooling.Instance.StopAll();
        int starResult = UIManager.Instance.TargetProcessUI.GetCurrentStars();
        UIManager.Instance.GameOverUI.InitializeEndGameUI(starResult);
        ChangeState(GameState.GameOver);
        //Time.timeScale = 0;
    }

    public void PlayAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
