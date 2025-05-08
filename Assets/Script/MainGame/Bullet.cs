using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using JetBrains.Annotations;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifeTime = 0.8f;
    [Networked] private NetworkBool didHitSomething { get; set; }
    [Networked] private TickTimer lifeTimeTimer { get; set; }
    private Collider2D coll;
    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
        coll = GetComponent<Collider2D>();
        lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, bulletLifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (!didHitSomething)
        {
            CheckIfHitGround();
            CheckIfHitAPlayer();
        }

        if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false)
        {
            transform.Translate(transform.right * bulletSpeed * Runner.DeltaTime, Space.World);
        }

        if (lifeTimeTimer.Expired(Runner) || didHitSomething)
        {
            lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    private void CheckIfHitGround()
    {
        var groundCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position, coll.bounds.size, 0f, groundLayerMask);

        if (groundCollider != default)
        {
            didHitSomething = true;
        }
    }

    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private void CheckIfHitAPlayer()
    {
        Debug.Log(coll.bounds.size);
        Debug.Log(Object.InputAuthority);
        Debug.Log(Runner.LagCompensation);
        Runner.LagCompensation.OverlapBox(transform.position, coll.bounds.size, Quaternion.identity, Object.InputAuthority, hits, playerLayerMask);
        if (hits.Count > 0)
        {
            foreach(var item in hits)
            {
                if (item.Hitbox != null)
                {
                    var player = item.Hitbox.GetComponentInParent<PlayerController>();
                    var didNotHitOurOwnPlayer = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                    if (didNotHitOurOwnPlayer)
                    {
                        if (Runner.IsServer)
                        {
                            player.GetComponent<PlayerHealthController>().Rpc_ReducePlayerHealth(bulletDamage);
                        }
                        didHitSomething = true;
                        break;
                    }
                }
            }
        }
    }
}
