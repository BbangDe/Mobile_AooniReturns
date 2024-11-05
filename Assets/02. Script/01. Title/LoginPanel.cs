using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : UIBase
{
    [SerializeField] Button loginBtn;
    [SerializeField] Button signUpBtn;
    
    [SerializeField] TMP_InputField IDInput;
    [SerializeField] TMP_InputField passwordInput;

    public override void Init()
    {
        loginBtn.onClick.AddListener(OnClickLogin);
        signUpBtn.onClick.AddListener(OnClickSignUp);

        passwordInput.contentType = TMP_InputField.ContentType.Password;
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        IDInput.text = string.Empty;

        passwordInput.text = string.Empty;
        passwordInput.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnClickLogin()
    {
        var logID = IDInput.text;
        var logPW = passwordInput.text;

        if (App.Manager.Title.TryLogin(logID, logPW, LoginErrorCallback, LoadErrorCallback))
        {
            App.UI.Title.GetPanel<LoadingPanel>().OpenPanel();
        }
    }

    private void OnClickSignUp()
    {
        App.UI.Title.GetPanel<SignUpPanel>().OpenPanel();
        ClosePanel();
    }

    private void LoginErrorCallback(ELoginError error)
    {
        switch (error)
        {
            case ELoginError.InvalidID:
                //InputFieldErrorEff(LoginIDInput.transform);
                break;

            case ELoginError.InvalidPW:
                //InputFieldErrorEff(LoginPWInput.transform);
                break;

            default:
                //ErrorNotice($"오류가 발생했습니다.({error})");
                break;
        }
    }

    private void LoadErrorCallback(ELoadError error)
    {
        // TODO: add error messages
        switch (error)
        {
            case ELoadError.InvalidVersion:
                // this client invalid, need update latest version.
                //Notice("STR_TITLE_VERSION_INVALID", Application.Quit); // force quit game.
                break;

            case ELoadError.TitleDataFail:
                //ErrorNotice($"타이틀 데이터를 가져올 수 없습니다.({error})");
                break;

            case ELoadError.PlayerDataFail:
                //ErrorNotice($"플레이어 데이터를 가져올 수 없습니다.({error})");
                break;

            default:
                //ErrorNotice($"오류가 발생했습니다.({error})");
                break;
        }
    }
}
