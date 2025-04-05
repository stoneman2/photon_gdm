using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    public static GlobalManagers Instance { get; private set; }
    [field: SerializeField] private DDOL parentObject;
    [field: SerializeField] public NetworkRunnerController NetworkRunnerController { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(parentObject.gameObject);
        }
    }
}