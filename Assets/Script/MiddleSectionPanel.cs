using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiddleSectionPanel : LobbyPanelBase
{
    [Header("MiddleSectionPanel")]
    public Button joinRandomRoomButton;
    public Button joinRandomByArgButton;
    public Button createRoomButton;

    public TMP_InputField joinRoomByArgInputField;
    public TMP_InputField createRoomInputField;

    public override void InitPanel(LobbyUIManager UIManager)
    {
        base.InitPanel(UIManager);
        joinRandomRoomButton.onClick.AddListener(OnClickJoinRandomRoom);
        joinRandomByArgButton.onClick.AddListener(OnClickJoinRandomByArg);
        createRoomButton.onClick.AddListener(CreateRoom);
    }

    private void CreateRoom()
    {
        if (createRoomInputField.text.Length >= 2)
        {
            // Create room here!
        }
    }

    private void OnClickJoinRandomRoom()
    {
    }

    private void OnClickJoinRandomByArg()
    {
    }
}
