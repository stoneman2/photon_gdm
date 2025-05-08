using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] LoadingCanvasController loadingCanvasControllerPrefab;
    [SerializeField] LobbyPanelBase[] lobbyPanels;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var lobby in lobbyPanels)
        {
            lobby.InitPanel(this);
        }
        Instantiate(loadingCanvasControllerPrefab);
    }

    public void ShowPanel(LobbyPanelBase.LobbyPanelType type)
    {
        foreach(var lobby in lobbyPanels)
        {
            if (lobby.PanelType == type)
            {
                lobby.ShowPanel();
                break;
            }
        }
    }
}
