using Fusion;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef PlayerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform[] spawnPoint;
    public override void Spawned()
    {
        // base.Spawned();
        if (Runner.IsServer)
        {
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
    }
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    void SpawnPlayer(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            int index = player.AsIndex % spawnPoint.Length;
            var spawnPoint = spawnPoint[player].transform.position;
            Runner.Spawn(PlayerNetworkPrefab, Vector3.zero, Quaternion.identity.player);
        }
    }
}
