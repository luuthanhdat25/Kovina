using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private TargetProcessUI targetProcessUI;
    public TargetProcessUI TargetProcessUI => targetProcessUI;

    [SerializeField]
    private PauseUI pauseUI;
    public PauseUI PauseUI => pauseUI;

    [SerializeField]
    private GameOverUI gameOverUI;
    public GameOverUI GameOverUI => gameOverUI;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        switch (e.NewGameState)
        {
            case GameManager.GameState.GamePlaying:
                pauseUI.BlackBackground.gameObject.SetActive(false);
                pauseUI.PauseMenu.gameObject.SetActive(false);

                break;

            case GameManager.GameState.GameOver:
                //inPlayingUI.ShowUI(false);
                break;

            case GameManager.GameState.GamePaused:
                Debug.Log(pauseUI.PauseMenu == null);
                pauseUI.PauseMenu.gameObject.SetActive(true);
                pauseUI.BlackBackground.gameObject.SetActive(true);
                //inPlayingUI.ShowUI(false);
                break;
        }
    }
}

