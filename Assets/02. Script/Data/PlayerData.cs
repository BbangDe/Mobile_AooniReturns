using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public enum EPlayerDataError
{
    LoadPlayerDataFailed,

    LoadProfileFailed,
    LoadUserDataFailed,
    UpdatePlayerStatisticsFailed,
    UpdateUserDataFailed,

    InvalidNickname,
    InvalidNicknameLength,
    LoadLeaderboardFailed,

    CreateGroupFailed,
    JoinGroupFailed,
    LeaveGroupFailed,
    ListGroupMembersFailed,
    GetGroupFailed,
    DeleteGroupFailed
}

public class PlayerData : Manager
{
    private string DefaultNick => $"아오오니{new Random().Next(1000, 10000)}";

    private Dictionary<string, UserDataRecord> roData;
    private Dictionary<string, UserDataRecord> userData;
    public IReadOnlyDictionary<string, UserDataRecord> UserData => userData;
    
    public string PlayerID { get; private set; }

    private string nickName;
    private string clan;
    private int experiencePoints;
    private int oniKills;
    private int hiroshiKills;
    private int survivalCount;
    private int currency;
    private int humanSkinIndex;
    private int oniSkinIndex;

    public string NickName => nickName;
    public string Clan => clan;
    public int ExperiencePoints => experiencePoints;
    public int OniKills => oniKills;
    public int HiroshiKills => hiroshiKills;
    public int SurvivalCount => survivalCount;
    public int Currency => currency;
    public int HumanSkinIndex => humanSkinIndex;
    public int OniSkinIndex => oniSkinIndex;

    private enum ENickValidateResult : byte
    {
        Invalid,
        Valid,
        ShouldUpdate
    }

    public void GetPlayerData(Action _getPlayerDataCallback, Action<EPlayerDataError> _errorHandler)
    {
        PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest()
        {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true,
                GetPlayerStatistics = true,
                GetUserReadOnlyData = true,
                GetUserData = true,
            }
        },
        result =>
        {
            var payload = result.InfoResultPayload;

            PlayerID = payload.PlayerProfile.PlayerId;

            UpdatePlayerProfileInternal(payload.PlayerProfile, null, payload.UserReadOnlyData);
            UpdateUserDataInternal(payload.UserData);
            UpdatePlayerStatisticsInternal(payload.PlayerStatistics);

            _getPlayerDataCallback?.Invoke();
        },
        error =>
        {
            _errorHandler?.Invoke(EPlayerDataError.LoadPlayerDataFailed);
        });
    }

    #region Profile

    public void GetPlayerProfile(Action _getProfileCallback, Action<EPlayerDataError> _errorHandler)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result =>
        {
            UpdatePlayerProfileInternal(result.PlayerProfile, _errorHandler);
            _getProfileCallback?.Invoke();
        },
        error =>
        {
            _errorHandler?.Invoke(EPlayerDataError.LoadProfileFailed);
        });
    }

    private void UpdatePlayerProfileInternal(PlayerProfileModel _profileData, Action<EPlayerDataError> _errorHandler, Dictionary<string, UserDataRecord> _roData = null)
    {
        if (_roData != null)
        {
            roData = _roData;
        }

        UpdatePlayerNickInternal(_profileData.DisplayName, _errorHandler);
    }

    #endregion

    #region Nickname

    public void SetPlayerNickName(string _nickName, Action<string> _setPlayerNickNameCallback, Action<EPlayerDataError> _errorHandler)
    {
        switch (ValidateNickInput(ref _nickName, _errorHandler))
        {
            case ENickValidateResult.Invalid:
                return;

            case ENickValidateResult.ShouldUpdate:
            case ENickValidateResult.Valid:
                break;
        }

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = _nickName,
        },
        result =>
        {
            UpdatePlayerNickInternal(result.DisplayName, _errorHandler);
            _setPlayerNickNameCallback?.Invoke(nickName);
        },
        error =>
        {
            _errorHandler?.Invoke(EPlayerDataError.InvalidNickname);
        });
    }

    private void UpdatePlayerNickInternal(string _nickname, Action<EPlayerDataError> _errorHandler)
    {
        nickName = _nickname;

        switch (ValidateNickData(ref nickName))
        {
            case ENickValidateResult.Invalid:
                nickName = DefaultNick;
                goto case ENickValidateResult.ShouldUpdate;

            case ENickValidateResult.ShouldUpdate:
                SetPlayerNickName(nickName, null, _errorHandler);
                break;

            case ENickValidateResult.Valid:
            default: break;
        }
    }

    private ENickValidateResult ValidateNickInput(ref string _nickName, Action<EPlayerDataError> _errorHandler)
    {
        if (_nickName == null)
        {
            _errorHandler?.Invoke(EPlayerDataError.InvalidNicknameLength);
            return ENickValidateResult.Invalid;
        }

        if (_nickName.Length < 1 || _nickName.Length > 8)
        {
            _errorHandler?.Invoke(EPlayerDataError.InvalidNicknameLength);
            return ENickValidateResult.Invalid;
        }

        return ENickValidateResult.Valid;
    }

    private ENickValidateResult ValidateNickData(ref string _nickName)
    {
        if (_nickName == null || _nickName.Length < 1 || _nickName.Length > 10)
        {
            return ENickValidateResult.Invalid;
        }

        return ENickValidateResult.Valid;
    }

    #endregion

    #region Player Statistics (ExperiencePoints, OniKills, HiroshiKills, SurvivalCount)

    private void UpdatePlayerStatisticsInternal(List<StatisticValue> statistics)
    {
        foreach (var stat in statistics)
        {
            switch (stat.StatisticName)
            {
                case "ExperiencePoints":
                    experiencePoints = stat.Value;
                    break;
                case "OniKills":
                    oniKills = stat.Value;
                    break;
                case "HiroshiKills":
                    hiroshiKills = stat.Value;
                    break;
                case "SurvivalCount":
                    survivalCount = stat.Value;
                    break;
            }
        }
    }

    public void GetPlayerStatistics(Action<Dictionary<string, int>> onSuccess, Action<PlayFabError> onError)
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
        result =>
        {
            Dictionary<string, int> playerStats = new Dictionary<string, int>();

            foreach (var stat in result.Statistics)
            {
                playerStats.Add(stat.StatisticName, stat.Value); 
            }

            onSuccess?.Invoke(playerStats);
        },
        error =>
        {
            onError?.Invoke(error);
        });
    }

    public void SetPlayerStatistics(int _experiencePoints, int _oniKills, int _hiroshiKills, int _survivalCount, Action onSuccess, Action<PlayFabErrorCode> onError)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "ExperiencePoints", Value = _experiencePoints },
                new StatisticUpdate { StatisticName = "OniKills", Value = _oniKills },
                new StatisticUpdate { StatisticName = "HiroshiKills", Value = _hiroshiKills },
                new StatisticUpdate { StatisticName = "SurvivalCount", Value = _survivalCount },
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
        result =>
        {
            experiencePoints = _experiencePoints;
            oniKills = _oniKills;
            hiroshiKills = _hiroshiKills;
            survivalCount = _survivalCount;

            SetExperiencePoints(experiencePoints.ToString(), null, null);

            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(error.Error);
        });
    }

    #endregion

    #region Title data (ExperiencePoints, Clan, Currency, HumanSkinIndex, OniSkinIndex)

    private void UpdateUserDataInternal(Dictionary<string, UserDataRecord> _userData)
    {
        userData = _userData;

        if (userData.ContainsKey("Clan"))
        {
            clan = userData["Clan"].Value;
        }
        if (userData.ContainsKey("Currency"))
        {
            int.TryParse(userData["Currency"].Value, out currency);
        }
        if (userData.ContainsKey("HumanSkinIndex"))
        {
            int.TryParse(userData["HumanSkinIndex"].Value, out humanSkinIndex);
        }
        if (userData.ContainsKey("OniSkinIndex"))
        {
            int.TryParse(userData["OniSkinIndex"].Value, out oniSkinIndex);
        }
    }

    public void SetExperiencePoints(string _exp, Action onSuccess, Action<EPlayerDataError> onError)
    {
        var requestData = new Dictionary<string, string>
        {
            { "ExperiencePoints", _exp }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = requestData
        },
        result =>
        {
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.UpdateUserDataFailed);
        });
    }

    public void SetClan(string _clan, Action onSuccess, Action<EPlayerDataError> onError)
    {
        var requestData = new Dictionary<string, string>
        {
            { "Clan", _clan }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = requestData
        },
        result =>
        {
            clan = _clan;
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.UpdateUserDataFailed);
        });
    }

    public void SetCurrency(int _currency, Action onSuccess, Action<EPlayerDataError> onError)
    {
        var requestData = new Dictionary<string, string>
        {
            { "Currency", _currency.ToString() }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = requestData
        },
        result =>
        {
            currency = _currency;
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.UpdateUserDataFailed);
        });
    }

    public void SetOniSkinIndex(int _oniSkinIndex, Action onSuccess, Action<EPlayerDataError> onError)
    {
        var requestData = new Dictionary<string, string>
        {
            { "OniSkinIndex", _oniSkinIndex.ToString() }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = requestData
        },
        result =>
        {
            oniSkinIndex = _oniSkinIndex;
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.UpdateUserDataFailed);
        });
    }

    public void SetHumanSkinIndex(int _humanSkinIndex, Action onSuccess, Action<EPlayerDataError> onError)
    {
        var requestData = new Dictionary<string, string>
        {
            { "HumanSkinIndex", _humanSkinIndex.ToString() },
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = requestData
        },
        result =>
        {
            humanSkinIndex = _humanSkinIndex;
            onSuccess?.Invoke();
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.UpdateUserDataFailed);
        });
    }
    #endregion

    #region Leaderboard
    public void GetLeaderboard(string statisticName, int maxResultsCount, Action<List<PlayerLeaderboardEntry>> onSuccess, Action<EPlayerDataError> onError)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = maxResultsCount,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowStatistics = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(request,
        result =>
        {
            if (result.Leaderboard != null && result.Leaderboard.Count > 0)
            {
                onSuccess?.Invoke(result.Leaderboard);
            }
            else
            {
                onError?.Invoke(EPlayerDataError.LoadLeaderboardFailed);
            }
        },
        error =>
        {
            onError?.Invoke(EPlayerDataError.LoadLeaderboardFailed);
        });
    }

    public void GetLeaderboardRank(string playFabId, Action<int, bool> onSuccess, Action<PlayFabError> onError)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "ExperiencePoints",
            PlayFabId = playFabId,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            if (result.Leaderboard != null && result.Leaderboard.Count > 0)
            {
                var playerRank = result.Leaderboard[0].Position;
                onSuccess?.Invoke(playerRank, true); 
            }
            else
            {
                onSuccess?.Invoke(-1, false); 
            }
        },
        error =>
        {
            onError?.Invoke(error); 
    });
    }
    #endregion
}