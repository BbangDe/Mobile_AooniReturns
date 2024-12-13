using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignUpPanel : UIBase
{
    [SerializeField] Button confirmBtn;
    [SerializeField] Button cancelBtn;

    [SerializeField] TMP_InputField IDInput;
    [SerializeField] TMP_InputField nickInput;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField checkInput;

    public override UIState GetUIState() => UIState.SignUp;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        confirmBtn.onClick.AddListener(OnClickConfirm);
        cancelBtn.onClick.AddListener(ClosePanel);

        passwordInput.contentType = TMP_InputField.ContentType.Password;
        checkInput.contentType = TMP_InputField.ContentType.Password;
    }


    public override void ClosePanel()
    {
        base.ClosePanel();

        App.UI.Title.GetPanel<LoginPanel>().OpenPanel();

        IDInput.text = "";

        nickInput.text = "";
        emailInput.text = "";

        passwordInput.text = "";
        passwordInput.contentType = TMP_InputField.ContentType.Password;

        checkInput.text = "";
        checkInput.contentType = TMP_InputField.ContentType.Password;
    }

    private void OnClickConfirm()
    {
        var regID = IDInput.text;
        var regPW = passwordInput.text;
        var regPWCheck = checkInput.text;
        var regNick = nickInput.text;
        var regEmail = emailInput.text;

        if (App.Manager.Title.TrySignUp(regID, regPW, regPWCheck, regNick, regEmail,
            SignUpErrorCallback, LoginErrorCallback, LoadErrorCallback))
        {
            ClosePanel();
        }
    }

    private void SignUpErrorCallback(ERegisterError error)
    {
        switch (error)
        {
            case ERegisterError.InvalidID:
                //InputFieldErrorEff(RegisterIDInput.transform);
                break;

            case ERegisterError.InvalidPW:
                //InputFieldErrorEff(RegisterPWInput.transform);
                break;

            case ERegisterError.InvalidPWCheck:
                //InputFieldErrorEff(RegisterPWCheckInput.transform);
                break;

            case ERegisterError.InvalidNick:
                //InputFieldErrorEff(RegisterNickInput.transform);
                break;

            default:
                //ErrorNotice($"오류가 발생했습니다.({error})");
                break;
        }
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
