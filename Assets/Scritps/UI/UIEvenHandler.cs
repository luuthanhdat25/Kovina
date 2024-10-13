using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvenHandler
{
    public void ChangUIApearence(GameObject currentScene, GameObject intoScene)
    {
        currentScene.SetActive(false);
        intoScene.SetActive(true);
    }
}
