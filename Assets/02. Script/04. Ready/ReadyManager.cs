using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class ReadyManager : SimManager, IPlayerJoined, IPlayerLeft
{
    [Header("Network Objects")]
    [SerializeField] NetworkObject playerPrefab;

    private void Start()
    {
        Runner.SpawnAsync(playerPrefab, Vector3.zero + (Vector3.left * 0.5f) + (Vector3.up * 0.5f), Quaternion.identity,
            Runner.LocalPlayer, null, NetworkSpawnFlags.SharedModeStateAuthLocalPlayer);
    }

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        App.UI.Ready.SetPlayerCount();
    }

    void IPlayerLeft.PlayerLeft(PlayerRef _player)
    {
        App.UI.Ready.SetPlayerCount();
    }
}
