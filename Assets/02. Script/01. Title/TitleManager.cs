using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Fusion;

public enum ELoginError
{
    None,
    AccountNotExist,
    AuthTokenGenerateFailed,
    AuthFailed,
    CloudConnectFailed,
    InvalidID,
    InvalidPW,
    InvalidGuestID,
    InvalidGuestPW,
}

public enum ERegisterError
{
    None,
    InvalidID,
    InvalidPW,
    InvalidPWCheck,
    InvalidNick,
    InvalidEmail,
}

public enum ELoadError
{
    None,
    InvalidVersion,
    TitleDataFail,
    PlayerDataFail,
}

public class TitleManager : ViewManager
{
    [SerializeField] bool autoLogin;

    private readonly string defaultEmail = "@aooni.com";

    private bool isConnectionBusy = false;
    private bool isLoggedIn = false;

    private void Start()
    {
        App.Manager.Sound.PlayBGM("BGM_Title");
        Application.targetFrameRate = 120;

        if (autoLogin)
        {
#if UNITY_EDITOR
            TryLogin("daun1124", "123456", null, null);
#else
            TryLogin("ekdms", "123456", null, null);
#endif
        }
    }

    public bool TryLogin(string ID, string PW,
        Action<ELoginError> _loginErrorHandler,
        Action<ELoadError> _loadErrorHandler)
    {
        if (isLoggedIn || isConnectionBusy)
        {
            return false;
        }

        if (ID == null || ID.Length < 3)
        {
            _loginErrorHandler(ELoginError.InvalidID);
            return false;
        }

        if (PW == null || PW.Length < 6)
        {
            _loginErrorHandler(ELoginError.InvalidPW);
            return false;
        }

        Login(ID, PW, () =>
        {
            LoadData(() =>
            {
                App.LoadScene(SceneName.Notice);
            },
            _loadErrorHandler);
        },
        _loginErrorHandler);

        return true;
    }

    private void Login(string ID, string PW,
       Action _loginCallback,
       Action<ELoginError> _loginErrorHandler)
    {
        if (isLoggedIn || isConnectionBusy)
        {
            return;
        }
        isConnectionBusy = true;

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = ID + defaultEmail,
            Password = PW,
        },
        (result) =>
        {
            isConnectionBusy = false;
            isLoggedIn = true;
            _loginCallback();
        },
        (error) =>
        {
            isConnectionBusy = false;
            Debug.LogError(error.Error);

            switch (error.Error)
            {
                case PlayFabErrorCode.AccountNotFound:
                    _loginErrorHandler(ELoginError.AccountNotExist);
                    break;

                case PlayFabErrorCode.InvalidUsername:
                case PlayFabErrorCode.InvalidEmailAddress:
                    _loginErrorHandler(ELoginError.InvalidID);
                    break;

                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.InvalidPassword:
                    _loginErrorHandler(ELoginError.InvalidPW);
                    break;

                default:
                    _loginErrorHandler((ELoginError)error.Error);
                    break;
            }
        });
    }

    private void LoadData(Action _loadDataCallback, Action<ELoadError> _loadDataErrorHandler)
    {
        if (isConnectionBusy)
        {
            return;
        }
        isConnectionBusy = true;
        
        App.Data.Title.LoadTitleData(() =>
        {
            App.Data.Player.GetPlayerData(() =>
            {
                _loadDataCallback?.Invoke();
                isConnectionBusy = false;
            },
            error =>
            {
                isConnectionBusy = false;
                _loadDataErrorHandler(ELoadError.PlayerDataFail);
            });
        },
        (error) =>
        {
            isConnectionBusy = false;
            Debug.LogError(error.Error);

            switch (error.Error)
            {
                case PlayFabErrorCode.VersionNotFound:
                    _loadDataErrorHandler(ELoadError.InvalidVersion);
                    break;

                default:
                    _loadDataErrorHandler((ELoadError)error.Error);
                    break;
            }
        });
    }

#region Sign Up
    public bool TrySignUp(string ID, string PW, string PWCheck, string Nick, string Email,
        Action<ERegisterError> _registerErrorHandler,
        Action<ELoginError> _loginErrorHandler,
        Action<ELoadError> _loadErrorHandler)
    {
        if (isConnectionBusy)
        {
            return false;
        }

        if (ID == null || ID.Length < 2)
        {
            _registerErrorHandler(ERegisterError.InvalidID);
            return false;
        }

        if (PW == null || PW.Length < 6)
        {
            _registerErrorHandler(ERegisterError.InvalidPW);
            return false;
        }

        if (PWCheck == null || PWCheck.Length < 6)
        {
            _registerErrorHandler(ERegisterError.InvalidPWCheck);
            return false;
        }

        if (Nick == null || Nick.Length < 3)
        {
            _registerErrorHandler(ERegisterError.InvalidNick);
            return false;
        }

        if (Email == null || Email.Length < 3)
        {
            _registerErrorHandler(ERegisterError.InvalidNick);
            return false;
        }

        if (!PWCheck.Equals(PW))
        {
            _registerErrorHandler(ERegisterError.InvalidPWCheck);
            return false;
        }

        SignUp(ID, PW, Nick, () =>
        {
            //App.UI.Title.GetPanel<AlertPanel>().ShowNotice("STR_NOTICE_CONPLETE_JOIN", () =>
            //{
            //    TryLogin(ID, PW, _loginErrorHandler, _loadErrorHandler);
            //});
        },
        _registerErrorHandler);

        return true;
    }

    private void SignUp(string ID, string PW, string nick,
        Action _signUpCallback,
        Action<ERegisterError> _signUpErrorHandler)
    {
        if (isConnectionBusy)
        {
            Debug.Log("isConnectionBusy");
            return;
        }
        isConnectionBusy = true;

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Email = ID + defaultEmail,
            Username = ID,
            Password = PW,
            DisplayName = nick,
        },
        (result) =>
        {
            Debug.Log("Success");
            _signUpCallback();
            isConnectionBusy = false;
        },
        (error) =>
        {
            isConnectionBusy = false;
            Debug.LogError(error.Error);

            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidUsername:
                case PlayFabErrorCode.InvalidEmailAddress:
                case PlayFabErrorCode.EmailAddressNotAvailable:
                    _signUpErrorHandler(ERegisterError.InvalidID);
                    break;

                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.InvalidPassword:
                    _signUpErrorHandler(ERegisterError.InvalidPW);
                    break;

                default:
                    _signUpErrorHandler((ERegisterError)error.Error);
                    break;
            }
        });
    }
#endregion
}