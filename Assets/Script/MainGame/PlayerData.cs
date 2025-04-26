using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct PlayerData : INetworkInput
{
    public float horizontalInput;
    public Quaternion gunPivotRotation;
    public NetworkButtons networkButtons;
}
