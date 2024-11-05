using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPanelButton : MonoBehaviour
{
    public string PanelId;

    private PanelController _manager;

    void Start()
    {
        _manager = PanelController.Instance;
    }

    public void DoShowPanel()
    {
        _manager.ShowPanel(PanelId);
    }
}
