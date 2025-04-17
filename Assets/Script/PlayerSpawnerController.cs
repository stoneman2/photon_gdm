using Fusion;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef PlayerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform[] spawnPoint;
    // public override void Spawned()
    // {
    //     // base.Spawned();
    //     if (Runner.IsServer)
    //     {
    //         foreach (var item in Runner.ActivePlayers)
    //         {
    //             SpawnPlayer(item);
    //         }
    //     }
    // }
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    void SpawnPlayer(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            int index = player.AsIndex % spawnPoint.Length;
            var newSpawnPoint = this.spawnPoint[index].transform.position;
            var PlayerObject = Runner.Spawn(PlayerNetworkPrefab, newSpawnPoint, Quaternion.identity, player);

            Runner.SetPlayerObject(player, PlayerObject);
        }
    }
}
