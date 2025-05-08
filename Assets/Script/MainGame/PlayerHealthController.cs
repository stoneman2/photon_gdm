using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private LayerMask deathGroundLayerMask;
    [SerializeField] private Animator bloodScreenHitAnimator;
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Image fillAmountImg;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [Networked]
    private int currentHealthAmount { get; set; }
    private const int MAX_HEALTH_AMOUNT = 100;
    private PlayerController playerController;
    private Collider2D coll;
    private ChangeDetector _changeDetector;
    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        coll = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer && playerController.PlayerIsAlive)
        {
            var didHitCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll.bounds.size, 0f, deathGroundLayerMask);
            if (didHitCollider != default)
            {
                Rpc_ReducePlayerHealth(MAX_HEALTH_AMOUNT);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    public override void Render()
    {
        foreach(var change in _changeDetector.DetectChanges(this, out var prev, out var current))
        {
            switch (change)
            {
                case nameof(currentHealthAmount):
                    var render = GetPropertyReader<int>(nameof(currentHealthAmount));
                    var (oldHealth, currentHealth) = render.Read(prev, current);
                    HealthAmountChange(oldHealth, currentHealth);
                    break;   
            }
        }
    }

    private void HealthAmountChange(int oldHealth, int currentHealth)
    {
        if (currentHealth != oldHealth)
        {
            UpdateVisuals(currentHealth);
            if (currentHealth != MAX_HEALTH_AMOUNT)
            {
                PlayerGotHit(currentHealth);
            }
        }
    }

    private void PlayerGotHit(int healthAmount)
    {
        if (Utils.IsLocalPlayer(Object))
        {
            // Shake the camera
            // playerCameraController.ShakeCamera(new Vector3(0.1f, 0.1f, 0f));
        }

        if (healthAmount <= 0)
        {
            playerController.KillPlayer();
        }
    }

    private void UpdateVisuals(int healthAmount)
    {
        var num = (float)healthAmount / MAX_HEALTH_AMOUNT;
        fillAmountImg.fillAmount = num;
        healthAmountText.text = $"{healthAmount}/{MAX_HEALTH_AMOUNT}";
    }

    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }
}
