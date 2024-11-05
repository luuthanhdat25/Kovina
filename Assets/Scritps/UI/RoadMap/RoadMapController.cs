using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoadMapController : MonoBehaviour
{
    [SerializeField] private Button backToMenuButton;

    void Start()
    {
        //backToMenuButton.onClick.AddListener(BackToMenu);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
