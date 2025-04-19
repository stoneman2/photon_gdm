using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.Experimental.RestService;
using TMPro;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject cam;
    public float moveSpeed = 6f;
    public float jumpForce = 1000f;
    Rigidbody2D rigid;
    [Networked] private NetworkButtons buttonPrev { get; set; }
    [Header("Ground Vars")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundDetectionObj;
    [Networked] private NetworkString<_8> playerName { get; set; }
    [Networked] private NetworkBool isGrounded { get; set; }
    private ChangeDetector changeDetector;
    float horizontal;
    public enum PlayerInputButtons
    {
        None,
        Jump,
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            rigid.velocity = new Vector2(input.horizontalInput * moveSpeed, rigid.velocity.y);
            CheckJumpInput(input);
        }
    }

    public override void Spawned()
    {
        rigid = GetComponent<Rigidbody2D>();   

        SetLocalObject();
    }

    private void SetLocalObject()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            cam.gameObject.SetActive(true);
            var nickName = GlobalManagers.Instance.NetworkRunnerController.LocalPlayerNickName;
            RpcSetNickName(nickName);
        }
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var prev, out var current))
        {
            switch(change)
            {
                case nameof(playerName):
                    var reader = GetPropertyReader<NetworkString<_8>>(nameof(playerName));
                    var (oldName, currentName) = reader.Read(prev, current);
                    OnNickNameChanged(currentName);
                    break;
            }
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RpcSetNickName(NetworkString<_8> nickName)
    {
        playerName = nickName;
    }

    private void OnNickNameChanged(NetworkString<_8> nickName)
    {
        SetPlayerNickName(nickName);
    }

    private void SetPlayerNickName(NetworkString<_8> nickName)
    {
        playerNameText.text = nickName + " " + Object.InputAuthority.PlayerId;
    }

    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            const string HORIZONTAL = "Horizontal";
            horizontal = Input.GetAxis(HORIZONTAL);
        }
    }

    public PlayerData GetPlayerNetworkInput()
    {
        PlayerData data = new PlayerData();
        data.horizontalInput = horizontal;
        data.networkButtons.Set(PlayerInputButtons.Jump, Input.GetKeyDown(KeyCode.Space));
        return data;
    }

    private void CheckJumpInput(PlayerData input)
    {
        var transform1 = groundDetectionObj.transform;
        isGrounded = (bool)Runner.GetPhysicsScene2D().OverlapBox(transform1.position, 
            transform1.localScale, 0f, groundLayers);

        if (isGrounded)
        {
            var pressed = input.networkButtons.GetPressed(buttonPrev);
            if (pressed.WasPressed(buttonPrev, PlayerInputButtons.Jump))
            {
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        buttonPrev = input.networkButtons;
    }
}
