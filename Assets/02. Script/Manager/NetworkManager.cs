using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public enum EScene : byte
{
    Title = 1,
    Lobby = 3,
    //Ready = 4,
    Game = 4,
    Result = 9,
}

[RequireComponent(typeof(INetworkSceneManager), typeof(INetworkObjectProvider))]
public class NetworkManager : Manager
{
    private INetworkSceneManager netSceneManager;
    private INetworkObjectProvider netObjectProvider;

    // NetworkRunnmer.CloudServices
    // CloudServices.CloudCommunicator
    // CloudCommnuicator.FusionRelayClient -> Realtime.LoadBalancingClient
    //public NetworkRunner Runner => App.Runner;
    public NetworkRunner Runner;
    public SessionInfo Session => App.Runner.SessionInfo;

    protected override void Awake()
    {
        base.Awake();
        
        netSceneManager = GetComponent<INetworkSceneManager>();
        netObjectProvider = GetComponent<INetworkObjectProvider>();
    }

    public void JoinLobby(Action _onComplete = null)
    {
        StartCoroutine(JoinLobbyInternal(_onComplete));
    }

    public void CreateMatch(string _roomName, string _password, ModeType _mode, Action _onComplete = null)
    {
        StartCoroutine(CreateMatchInternal(_roomName, _password, _mode, _onComplete));
    }

    public void JoinMatch(SessionInfo _info, Action _onComplete = null)
    {
        if(_info.PlayerCount < _info.MaxPlayers)
        {
            StartCoroutine(JoinMatchInternal(_info, _onComplete));
        }
    }

    public void LeaveMatch(Action _onComplete = null)
    {
        Runner.Shutdown(true);
        SceneManager.LoadScene((int)EScene.Lobby);

        try { _onComplete?.Invoke(); }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of LeaveMatch. " +
                $"{error.Message}\n{error.StackTrace}");
        }
    }

    public void StartGame(Action _onComplete = null)
    {
        StartCoroutine(StartGameInternal(_onComplete));
    }

    public void ShowResult(Action _onComplete = null)
    {
        //var elapsed = App.Manager.UI.GetPanel<Panel_Time>().Elapsed;
        //App.PlayerInfo.ElapsedMin = elapsed.Item1;
        //App.PlayerInfo.ElapsedSec = elapsed.Item2;

        StartCoroutine(ShowResultInternal(_onComplete));
    }

    private IEnumerator JoinLobbyInternal(Action _onComplete)
    {
        /*
        var joinTask = Runner.JoinSessionLobby(SessionLobby.Shared, "default");

        yield return new WaitUntil(() => joinTask.IsCompleted);

        var joinTaskResult = joinTask.Result;
        if (!joinTaskResult.Ok)
        {
            // TODO: handle error case
            Debug.LogError("Failed to join lobby. Exiting...");
            LeaveMatch();
            yield break;
        }*/

        SceneManager.LoadScene((int)EScene.Lobby);
        
        yield return null;
        /*
        try { _onComplete?.Invoke(); }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of JoinLobby. " +
                $"{error.Message}\n{error.StackTrace}");
        }*/
    }

    private IEnumerator CreateMatchInternal(string _roomName, string _password, ModeType _mode, Action _onComplete)
    {
        var joinTask = Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = _roomName,
            IsOpen = true,
            IsVisible = true,
            UseCachedRegions = true,
            SceneManager = netSceneManager,
            ObjectProvider = netObjectProvider,
            PlayerCount = GetMaxPlayers(_mode),
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "GameMode", (int)_mode },
            }
        });

        yield return new WaitUntil(() => joinTask.IsCompleted);

        var joinTaskResult = joinTask.Result;
        if (!joinTaskResult.Ok)
        {
            // TODO: handle error case
            Debug.LogError("Failed to join game. Exiting...");
            LeaveMatch();
            yield break;
        }

        //EDIT: Ready => Game (씬 통합) 
        SceneManager.LoadScene((int)EScene.Game);

        try { _onComplete?.Invoke(); }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of FindMatch. " +
                $"{error.Message}\n{error.StackTrace}");
        }
    }

    private int GetMaxPlayers(ModeType _type) => _type switch
    {
        ModeType.Infection => 8,
        ModeType.Bomb => 8,
        ModeType.Police => 8,
        ModeType.Dual => 2,
        _ => 8
    };

    private IEnumerator JoinMatchInternal(SessionInfo _info, Action _onComplete)
    {
        var gameMode = (int)_info.Properties["GameMode"];

        var joinTask = Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = _info.Name,
            IsOpen = _info.IsOpen,
            IsVisible = _info.IsVisible,
            UseCachedRegions = true,
            SceneManager = netSceneManager,
            ObjectProvider = netObjectProvider,
            PlayerCount = GetMaxPlayers((ModeType)gameMode),
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "GameMode", gameMode },
            }
        });

        yield return new WaitUntil(() => joinTask.IsCompleted);

        var joinTaskResult = joinTask.Result;
        if (!joinTaskResult.Ok)
        {
            // TODO: handle error case
            Debug.LogError("Failed to join game. Exiting...");
            LeaveMatch();
            yield break;
        }

        //EDIT: Ready => Game (씬 통합) 
        SceneManager.LoadScene((int)EScene.Game); 

        try { _onComplete?.Invoke(); }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of FindMatch. " +
                $"{error.Message}\n{error.StackTrace}");
        }
    }

    private IEnumerator StartGameInternal(Action _onComplete)
    {
        Runner.SessionInfo.IsOpen = false;
        var modeIndex = (int)Runner.SessionInfo.Properties["GameMode"];
        var sceneIndex = modeIndex + (int)EScene.Game;
        var loadTask = Runner.LoadScene(SceneRef.FromIndex(sceneIndex));

        yield return new WaitUntil(() => loadTask.IsDone);

        try { _onComplete?.Invoke(); }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of StartGame. " +
                $"{error.Message}\n{error.StackTrace}");
        }
    }

    private IEnumerator ShowResultInternal(Action _onComplete)
    {
        var loadTask = Runner.LoadScene(SceneRef.FromIndex((int)EScene.Result));

        yield return new WaitUntil(() => loadTask.IsDone);

        try { 
            _onComplete?.Invoke();
            App.Manager.Sound.PlaySFX("SFX_victory");
        }
        catch (Exception error)
        {
            Debug.LogError("Exception was thrown while invoking OnComplete of ShowResult. " +
                $"{error.Message}\n{error.StackTrace}");
        }
    }
}
