using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PanelController : Singleton<PanelController>
{

    [SerializeField] public List<PanelModel> panels;


    
    private Queue<PanelModel> _panelModelQueue = new Queue<PanelModel>();
    public void ShowPanel(string panelID) 
    {
        var panelModel = panels.FirstOrDefault(panel => panel.panelId == panelID);
        if(panelModel != null)
        {
            var newInstancePanel = Instantiate(panelModel.PanelPrefab,transform);
            newInstancePanel.transform.localPosition = Vector3.zero;

            _panelModelQueue.Enqueue(new PanelModel
            {
                panelId = panelID,
                PanelPrefab = newInstancePanel
            });

        }
        else
        {
            Debug.Log($"We don't have any panelID = {panelID}");
        }
    }
    
}


