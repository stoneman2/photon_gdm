using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNickNamePanel : LobbyPanelBase
{
    [Header("CreateNickNamePanel")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button createNickNameButton;
    public int MIN_NICK_LENGTH = 2;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void InitPanel(LobbyUIManager lobbyUIManager)
    {
        base.InitPanel(lobbyUIManager);
        createNickNameButton.interactable = false;
        createNickNameButton.onClick.AddListener(OnClickCreateNickName);
        inputField.onValueChanged.AddListener(OnInputValueChange);
    }

    private void OnInputValueChange(string args)
    {
        createNickNameButton.interactable = args.Length >= MIN_NICK_LENGTH;
    }

    private void OnClickCreateNickName()
    {
        var nickName = inputField.text;
        if (nickName.Length >= MIN_NICK_LENGTH)
        {
            lobbyUIManager.ShowPanel(LobbyPanelType.MiddleSectionPanel);
            ClosePanel();
        }
    }
}
