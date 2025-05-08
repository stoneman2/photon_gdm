using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject cam;
    public float moveSpeed = 6f;
    public float jumpForce = 1000f;
    Rigidbody2D rigid;
    private PlayerWeaponController playerWeaponController;
    private PlayerVisualController playerVisualController;
    private PlayerHealthController playerHealthController;
    [Networked] private NetworkButtons buttonPrev { get; set; }
    [Header("Ground Vars")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundDetectionObj;
    [Networked] private NetworkString<_8> playerName { get; set; }
    [Networked] public TickTimer respawnTimer { get; set; }
    [Networked] private NetworkBool isGrounded { get; set; }
    [Networked] public NetworkBool PlayerIsAlive { get; set; }
    [Networked] private TickTimer respawnToNewPointTimer { get; set; }
    [Networked] private Vector2 serverNextSpawnPoint { get; set; }
    private ChangeDetector _changeDetector;
    float horizontal;
    public enum PlayerInputButtons
    {
        None,
        Jump,
        Shoot,
    }

    public override void FixedUpdateNetwork()
    {
        CheckRespawnTimer();
        
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            rigid.velocity = new Vector2(input.horizontalInput * moveSpeed, rigid.velocity.y);
            CheckJumpInput(input);
        }

        playerVisualController.UpdateScaleTransforms(rigid.velocity);
    }

    public override void Spawned()
    {
        rigid = GetComponent<Rigidbody2D>();   
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState, false);
        playerWeaponController = GetComponent<PlayerWeaponController>();
        playerVisualController = GetComponent<PlayerVisualController>();
        playerHealthController = GetComponent<PlayerHealthController>();
        SetLocalObject();

        PlayerIsAlive = true;
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
        foreach (var change in _changeDetector.DetectChanges(this, out var prev, out var current))
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

        playerVisualController.RenderVisuals(rigid.velocity);
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
        data.gunPivotRotation = playerWeaponController.localQuaternionPivotRot;
        data.networkButtons.Set(PlayerInputButtons.Jump, Input.GetKeyDown(KeyCode.Space));
        data.networkButtons.Set(PlayerInputButtons.Shoot, Input.GetButtonDown("Fire1"));
        return data;
    }

    private void CheckRespawnTimer()
    {
        if (PlayerIsAlive) return;

        if (respawnToNewPointTimer.Expired(Runner))
        {
            GetComponent<NetworkRigidbody2D>().Teleport(serverNextSpawnPoint);
            respawnToNewPointTimer = TickTimer.None;
        }

        if (respawnTimer.Expired(Runner))
        {
            respawnTimer = TickTimer.None;
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        PlayerIsAlive = true;
        rigid.simulated = true;
        playerVisualController.TriggerRespawnAnimation();
        playerHealthController.ResetHealthAmountToMax();
    }

    public void KillPlayer()
    {
        const int RESPAWN_TIME = 5;
        if (Runner.IsServer)
        {
            serverNextSpawnPoint = GlobalManagers.Instance.playerSpawnerController.GetRandomSpawnPoint();
            respawnToNewPointTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_TIME - 1);
        }

        PlayerIsAlive = false;
        rigid.simulated = false;
        playerVisualController.TriggerDeathAnimation();
        respawnTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_TIME);
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
