using Fusion;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef PlayerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform[] spawnPoint;
    public override void Spawned()
    {
        // // base.Spawned();
        // if (Runner.IsServer)
        // {
        //     foreach (var item in Runner.ActivePlayers)
        //     {
        //         SpawnPlayer(item);
        //     }
        // }
    }

    public void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.playerSpawnerController = this;
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"Player {player} joined the game.");
        SpawnPlayer(player);
    }

    void SpawnPlayer(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            Debug.Log($"Spawning player {player}.");
            Debug.Log($"Player {player} index: {player.AsIndex}.");
            Debug.Log($"Spawn point length: {spawnPoint.Length}.");
            int index = player.AsIndex % spawnPoint.Length;
            var newSpawnPoint = this.spawnPoint[index].transform.position;
            var PlayerObject = Runner.Spawn(PlayerNetworkPrefab, newSpawnPoint, Quaternion.identity, player);

            Runner.SetPlayerObject(player, PlayerObject);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }

            Runner.SetPlayerObject(playerRef, null);
        }
    }

    public Vector2 GetRandomSpawnPoint()
    {
        var  index = Random.Range(0, spawnPoint.Length);
        return spawnPoint[index].position;
    }
}
