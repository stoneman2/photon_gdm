using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifeTime = 0.8f;
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
        if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false)
        {
            transform.Translate(transform.right * bulletSpeed * Runner.DeltaTime, Space.World);
        }

        if (lifeTimeTimer.Expired(Runner))
        {
            lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }
}
