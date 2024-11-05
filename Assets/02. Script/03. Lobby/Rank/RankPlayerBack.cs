using System;

using PlayFab;
using PlayFab.ClientModels;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankPlayerBack : MonoBehaviour
{
    [SerializeField] Image medalImg;

    [SerializeField] TextMeshProUGUI rankTMP;
    [SerializeField] TextMeshProUGUI levelTMP;
    [SerializeField] TextMeshProUGUI nickTMP;

    [SerializeField] TextMeshProUGUI scoreTMP;

    private string playerID = string.Empty;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Init(PlayerLeaderboardEntry _player, RankType _type)
    {
        if (playerID == _player.PlayFabId) return;

        rankTMP.text = string.Format("{0}위", (_player.Position + 1).ToString());
        nickTMP.text = _player.DisplayName;

        GetPlayerEXP(_player.PlayFabId,
        (result) =>
        {
            SetLevelTMP(result);
        }, null);

        scoreTMP.text = string.Format(GetScoreText(_type), _player.StatValue.ToString());

        playerID = _player.PlayFabId;
        gameObject.SetActive(true);
    }

    private void GetPlayerEXP(string playFabId, Action<int> onSuccess, Action<EPlayerDataError> onError)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };

        PlayFabClientAPI.GetUserData(request,
        result =>
        {
            if (result.Data.ContainsKey("ExperiencePoints"))
            {
                int.TryParse(result.Data["ExperiencePoints"].Value, out var exp);
                Debug.Log(result.Data["ExperiencePoints"].Value);
                onSuccess?.Invoke(exp);
            }
            else
            {
                Debug.LogError($"Failed to get user data for PlayFabId: {playFabId}. Error: LoadUserDataFailed");
            }
        },
        error =>
        {
            Debug.LogError($"Failed to get user data for PlayFabId: {playFabId}. Error: {error.GenerateErrorReport()}");
        });
    }

    private void SetLevelTMP(int _exp)
    {
        var result = CalculateLevel(_exp);

        levelTMP.text = string.Format("Lv.{0}", result.ToString());
    }

    private int CalculateLevel(int _totalExp)
    {
        int currLevel = 1;
        int requiredExp = 50;

        while (_totalExp >= requiredExp)
        {
            _totalExp -= requiredExp;
            currLevel++;
            requiredExp += 100;
        }

        return currLevel;
    }

    private string GetScoreText(RankType _type) => _type switch
    {
        RankType.Exp => "{0} EXP",
        _ => "{0}회"
    };
}
