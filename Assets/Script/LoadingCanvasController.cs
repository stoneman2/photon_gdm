using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Button cancelBtn;

    private NetworkRunnerController networkRunnerController;

    void Start()
    {
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
        networkRunnerController.OnStartRunnerConnection += OnStartRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully += OnPlayerJoinedSuccessfully;
        cancelBtn.onClick.AddListener(OnClickCancelBtn);   
        gameObject.SetActive(false);
    }

    void OnPlayerJoinedSuccessfully()
    {
        string CLIP_NAME = "Out";
        StartCoroutine(Utils.PlayAnimationSetStateFinish(gameObject, _animator, CLIP_NAME));
    }

    void OnStartRunnerConnection()
    {
        this.gameObject.SetActive(true);
        string CLIP_NAME = "In";
        StartCoroutine(Utils.PlayAnimationSetStateFinish(gameObject, _animator, CLIP_NAME));
    }

    void OnClickCancelBtn()
    {
        Debug.Log("Cancel button clicked!");
    }

    void OnDestroy()
    {
        networkRunnerController.OnStartRunnerConnection -= OnStartRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully -= OnPlayerJoinedSuccessfully;
    }
}
