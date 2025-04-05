using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
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

    NetworkRunnerController networkRunnerController;

    public override void InitPanel(LobbyUIManager UIManager)
    {
        base.InitPanel(UIManager);

        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;

        joinRandomRoomButton.onClick.AddListener(OnClickJoinRandomRoom);
        joinRandomByArgButton.onClick.AddListener(() =>
        {
            CreateRoom(GameMode.Client, joinRoomByArgInputField.text);
        });
        createRoomButton.onClick.AddListener(() =>
        {
            CreateRoom(GameMode.Host, createRoomInputField.text);
        });
    }

    private void CreateRoom(GameMode mode, string field)
    {
        if (field.Length >= 2)
        {
            Debug.Log($"============{mode}============");
            Debug.Log($"Create room with name: {field}");

            // Create room here!
            GlobalManagers.Instance.NetworkRunnerController.StartGame(mode, field);
        }
    }

    private void OnClickJoinRandomRoom()
    {
        Debug.Log("============Joined Random Room!============");
        GlobalManagers.Instance.NetworkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }

    private void OnClickJoinRandomByArg()
    {
    }
}
