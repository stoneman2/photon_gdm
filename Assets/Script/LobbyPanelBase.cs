using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    public LobbyPanelType PanelType;
    protected LobbyUIManager lobbyUIManager;
    [SerializeField] Animator PanelAnimator;
    // Start is called before the first frame update
    public enum LobbyPanelType
    {
        None,
        CreateNickNamePanel,
        MiddleSectionPanel
    }
    public virtual void InitPanel(LobbyUIManager UIManager)
    {
        lobbyUIManager = UIManager; 
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        string POP_IN_CLIPNAME = "In";
        PanelAnimator.Play(POP_IN_CLIPNAME);
    }

    public void ClosePanel()
    {
        string POP_OUT_CLIPNAME = "Out";
        PanelAnimator.Play(POP_OUT_CLIPNAME);
        StartCoroutine(Utils.PlayAnimationSetStateFinish(gameObject, PanelAnimator, POP_OUT_CLIPNAME, false));
    }
}
