using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Camera cam; 
    [field: SerializeField] public Collider2D CameraBounds { get; private set; }
    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }
    public override void Spawned()
    {
        cam.gameObject.SetActive(false);
    }
}
