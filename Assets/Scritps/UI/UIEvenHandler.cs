using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvenHandler
{
   
    public IEnumerator ChangUIApearence(GameObject currentScene, GameObject intoScene)
    {
        yield return new WaitForSeconds(0.2f);
        currentScene.SetActive(false);
        intoScene.SetActive(true);
    }
}
