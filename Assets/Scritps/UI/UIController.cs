using UnityEngine;
using UnityEngine.SceneManagement;

public class UIContronller : MonoBehaviour
{

    private Animator _animator;
    private UIEvenHandler controller = new UIEvenHandler();
    public string panelId;
    [SerializeField] GameObject creditUI;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        creditUI.SetActive(false);
    }
    public void SetTriggerAnimation()
    {
        _animator.SetTrigger("IsMenuSet");
    }
    public void ActionMenuHandler(GameObject playerActionOption)
    {
        if (playerActionOption == GameObject.Find("PlayButton"))
        {
            Debug.Log("isPlay");
            SceneManager.LoadScene("Roadmap");
        }
        else if (playerActionOption == GameObject.Find("CreditButton"))
        {
            creditUI.SetActive(true);
        }
        else if (playerActionOption == GameObject.Find("ExitButton")) 
        {
            Debug.Log("Exited game");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    public void ChangeToSettingUI(GameObject setting)
    {
        var itemMenu = GameObject.Find("Panel_ItemSystem");
        StartCoroutine(controller.ChangUIApearence(itemMenu, setting));
    }
    public void RollBackToMenuUI(GameObject setting)
    {
        StartCoroutine(controller.ChangUIApearence(setting,this.gameObject));
    }
    public void CloseCredit()
    {
        creditUI.SetActive( false );
    }
}
