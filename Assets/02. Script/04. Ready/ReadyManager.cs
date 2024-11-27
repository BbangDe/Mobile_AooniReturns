using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class ReadyManager : SimManager, IPlayerJoined, IPlayerLeft
{
    [Header("Network Objects")]
    [SerializeField] NetworkObject playerPrefab;


    [SerializeField] InfectionManager infectionManager;
    [SerializeField] BombManager bombManager;
    [SerializeField] PoliceManager policeManager;
    [SerializeField] DualManager dualManager;

    private void Start()
    {
        var modeType = App.Manager.Network.Runner.SessionInfo.Properties["GameMode"];
        string modeTxt = GetModeName(modeType);

        switch (modeTxt)
        {
            case "감염":
                infectionManager.gameObject.SetActive(true);
                break;
            case "폭탄":
                bombManager.gameObject.SetActive(true);
                break;
            case "도둑과 경찰":
                policeManager.gameObject.SetActive(true);
                break;
            case "듀얼":
                dualManager.gameObject.SetActive(true);
                break;
        }

        App.Manager.Sound.StopBGM();
        App.Manager.Sound.PlaySFX("SFX_Game_Door");

        Runner.SpawnAsync(playerPrefab, Vector3.zero + (Vector3.left * 0.5f) + (Vector3.up * 0.4f), Quaternion.identity,
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

    private string GetModeName(int _modeIndex) => (ModeType)_modeIndex switch
    {
        ModeType.Infection => "감염",
        ModeType.Bomb => "폭탄",
        ModeType.Police => "도둑과 경찰",
        ModeType.Dual => "듀얼",
        _ => "감염"
    };
}
