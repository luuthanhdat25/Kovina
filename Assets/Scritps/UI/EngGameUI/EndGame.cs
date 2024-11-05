using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private int starNo;

    [SerializeField] private GameObject[] star;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] GameObject Victory;
    [SerializeField] GameObject Lost;
    [SerializeField] GameObject ButtonContainer;
    [SerializeField] Button backToRoadMap;
    [SerializeField] Button replayButton;

    void Start()
    {
        backToRoadMap.onClick.AddListener(() => { SceneManager.LoadScene(SceneEnum.Roadmap.ToString()); });
    }
    public void SetStarNo(int numberStar)
    {
        this.starNo = numberStar;
       
    }
    public void InitializeEndGameUI()
    {
        
        Debug.Log($"star number {this.starNo}");
        endGameUI.transform.localScale = Vector3.zero;
        ButtonContainer.transform.localScale = Vector3.zero;
        Victory.SetActive(false);
        Lost.SetActive(false);
        PlayEndGameUI();
        if (starNo > 0)
        {
            SoundManager.Instance.PlayVictorySound();
            Victory.SetActive(true); 
        }
        else
        {
            SoundManager.Instance.PlayLostSound();
            Lost.SetActive(true);
        }
    }

    private void ShowStar()
    {
        var sequence = LeanTween.sequence();
        for (int i = 0; i < starNo; i++)
        {
            int index = i;
            sequence.append(() => PlayStar(star[index]));
            sequence.append(0.6f);
        }
        sequence.append(() => PlayButton());
    }

    private void PlayStar(GameObject star)
    {
        LeanTween.scale(star, new Vector3(1, 1, 1), 0.5f);
        LeanTween.rotate(star, new Vector3(0, 0, 720), 0.5f);
    }

    private void PlayButton()
    {
        LeanTween.scale(ButtonContainer, new Vector3(1.75f, 1.75f, 1.75f), 0.5f);
    }

    private void PlayEndGameUI()
    {
        LeanTween.scale(endGameUI, new Vector3(0.3f, 0.3f, 0.3f), 0.5f)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(0.2f, () => ShowStar());
            });
    }
}
