using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PanelControl : MonoBehaviour
{
    // Start is called before the first frame update
    

    
    public  GameObject panelSettingWindow;
    public Image backgroundMenuImage;

    void Update()
    {
        if (panelSettingWindow.activeSelf)
        {
            backgroundMenuImage.color = new Color(125f / 255f, 125f / 255f, 125f / 255f, 1f);
        }
        else
        {
            backgroundMenuImage.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);
        }
    }
}
