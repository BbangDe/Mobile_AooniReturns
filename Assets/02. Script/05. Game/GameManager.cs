using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameState
{
    None,
    Begin,
    CountDown,
    Play,
    Over,
}

public abstract class GameManager : NetManager
{
    [Header("Network")]
    [SerializeField] NetworkObject netPlayerObject;
    [SerializeField] Transform respawnPos;

    public bool IsGamePlay { get; protected set; } = false;
    public int GameTime { get; protected set; }

    public int RoundCount { get; protected set; } = 0;
    public int MaxRoundCount { get; protected set; }

    private GameState currState = GameState.None;
    private float internalTime = 1;

    public override void Spawned()
    {
        App.Manager.Sound.PlayBGM("BGM_Game");

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        var spawnTask = Runner.SpawnAsync(netPlayerObject, respawnPos.position, respawnPos.rotation,
            Runner.LocalPlayer, null, NetworkSpawnFlags.SharedModeStateAuthLocalPlayer);

        yield return new WaitUntil(() => spawnTask.GetAwaiter().IsCompleted);

        if (spawnTask.IsFailed)
        {
            Debug.LogError("Unable to spawn player object. Quiting...");
            App.Manager.Network.LeaveMatch();
            yield break;
        }

        if (Runner.IsSceneAuthority)
        {
            // 뱌로 보내면 안감. 실패 이유 구글링해도 안나옴.
            // ObjectNotConfirmed라는데 서버에서 해주는건 있지도 않으면서 확인은 왜한다는건지 모르겠음
            // 도대체 얘내가 뭘 추구하고 뭘 위해서 이렇게 하는지도 모르겠음
            // TargetObjectVerificationResult.ObjectNotConfirmed
            yield return new WaitForSeconds(1f);
            StartCoroutine(InternalGameLoop());
        }
    }

    private IEnumerator InternalGameLoop()
    {
        do
        {
            internalTime -= Time.deltaTime;
            if (internalTime > 0f)
            {
                yield return null;
                continue;
            }

            var nextState = GetNextState(currState);

            RPC_ChangeState(nextState);

            internalTime = GetRequiredTime(nextState);

            currState = nextState;

            if ((int)internalTime < 0)
            {
                yield break;
            }

            yield return null;
        }
        while (true);
    }

    private GameState GetNextState(GameState _prevState)
    {
        var nextState = GameState.None;

        switch (_prevState)
        {
            case GameState.None:
                nextState = GameState.Begin;
                break;

            case GameState.Begin:
                nextState = GameState.CountDown;
                break;

            case GameState.CountDown:
                nextState = GameState.Play;
                break;

            case GameState.Play:
                if (RoundCount >= MaxRoundCount)
                {
                    nextState = GameState.Over;
                }
                else
                {
                    nextState = GameState.Begin;
                }
                break;
        }

        return nextState;
    }

    private float GetRequiredTime(GameState _time) => _time switch
    {
        GameState.Begin => 3,
        GameState.CountDown => 10,
        GameState.Play => GameTime,
        _ => -1,
    };

    [Rpc]
    private void RPC_ChangeState(GameState _state, RpcInfo _info = default)
    {
        currState = _state;

        switch (_state)
        {
            case GameState.None:
                IsGamePlay = false;
                Debug.LogError($"Impossible route detected. {_state}");
                break;

            case GameState.Begin:
                IsGamePlay = false;
                RoundCount++;

                if (RoundCount > 1)
                {
                    App.Manager.Player.SetAllHuman();
                }

                App.Manager.UI.GetPanel<RoundPanel>().OpenPanel();
                App.Manager.UI.GetPanel<TimePanel>().ClosePanel();
                App.Manager.UI.GetPanel<NoticePanel>().NoticeBeforeGameStart();
                break;

            case GameState.CountDown:
                IsGamePlay = false;
                App.Manager.UI.GetPanel<NoticePanel>().NoticeCountDown();
                break;

            case GameState.Play:
                App.Manager.UI.GetPanel<RoundPanel>().ClosePanel();
                App.Manager.UI.GetPanel<TimePanel>().OpenPanel();
                SetRandomOni();

                IsGamePlay = true;
                break;

            case GameState.Over:
                IsGamePlay = false;

                App.Manager.Network.ShowResult();
                break;
        }
    }

    private void SetRandomOni()
    {
        if (!Runner.IsSceneAuthority)
        {
            return;
        }

        App.Manager.Player.SetRandomOni();
    }

    public override void Render()
    {
        if (!Runner.IsSceneAuthority || !IsGamePlay)
        {
            return;
        }

        if (App.Manager.Player.AllPlayers.Count == 0)
        {
            return;
        }

        if (CheckVictoryCondition())
        {
            IsGamePlay = false;
            internalTime = 0;
        }
    }

    protected abstract bool CheckVictoryCondition();
}

