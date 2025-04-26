using Fusion;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private NetworkPrefabRef bulletPrefab;
    [SerializeField] private Transform firePointPos;
    [SerializeField] private float delayBetweenShots = 0.10f;
    [SerializeField] private ParticleSystem muzzleEffect;
    [SerializeField] private Camera localCam;
    [SerializeField] private Transform pivotToRotation;
    [Networked, HideInInspector] public NetworkBool isHoldingKey { get; private set; }
    [Networked] private NetworkBool playMuzzleEffect { get; set; }
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    [Networked] private TickTimer shootCooldown { get; set; }
    public Quaternion localQuaternionPivotRot { get; private set; }
    private PlayerController playerController;
    private ChangeDetector _changeDetector;
    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
        playerController = GetComponent<PlayerController>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            localQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            CheckShootInput(input);
            currentPlayerPivotRotation = input.gunPivotRotation;
            buttonsPrev = input.networkButtons;
        }
        else
        {
            isHoldingKey = false;
            playMuzzleEffect = false;
            buttonsPrev = default;
        }
        pivotToRotation.rotation = currentPlayerPivotRotation;
    }

    private void CheckShootInput(PlayerData input)
    {
        var currentButtons = input.networkButtons.GetPressed(buttonsPrev);
        if (currentButtons.WasReleased(buttonsPrev, PlayerController.PlayerInputButtons.Shoot)
            && shootCooldown.ExpiredOrNotRunning(Runner))
        {
            playMuzzleEffect = true;
            shootCooldown = TickTimer.CreateFromSeconds(Runner, delayBetweenShots);
            if (Runner.IsServer)
            {
                Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
            }
        }
        else
        {
            playMuzzleEffect = false;
        }
    }
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this, out var prev, out var current))
        {
            switch (change)
            {
                case nameof(playMuzzleEffect):
                    var render = GetPropertyReader<NetworkBool>(nameof(playMuzzleEffect));
                    var (oldState, currentState) = render.Read(prev, current);
                    PlayOrStopMuzzleEffect(currentState);
                    break;
            }
        }
    }
    private void PlayOrStopMuzzleEffect(bool play)
    {
        if (play)
        {
            muzzleEffect.Play();
        }
        else
        {
            muzzleEffect.Stop();
        }
    }
}
