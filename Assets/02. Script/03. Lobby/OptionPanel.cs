using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : UIBase
{
    [SerializeField] Button bgmBtn;
    [SerializeField] Button sfxBtn;
    [SerializeField] Button controlPositionBtn;
    [SerializeField] Button crossKeysBtn;

    [SerializeField] Button continueBtn;

    [SerializeField] TextMeshProUGUI bgmTMP;
    [SerializeField] TextMeshProUGUI sfxTMP;
    [SerializeField] TextMeshProUGUI controlPositionTMP;
    [SerializeField] TextMeshProUGUI crossKeysTMP;

    public override UIState GetUIState() => UIState.Option;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        SetSoundText(bgmTMP, App.Manager.Sound.IsMuted(AudioType.BGM));
        SetSoundText(sfxTMP, App.Manager.Sound.IsMuted(AudioType.SFX));

        bgmBtn.onClick.AddListener(OnClickBGM);
        sfxBtn.onClick.AddListener(OnClickSFX);

        continueBtn.onClick.AddListener(ClosePanel);
    }

    public override void OpenPanel()
    {
        if (App.UI.Lobby.CurrState == UIState.CreateRoom)
        {
            App.UI.Lobby.GetPanel<CreateRoomPanel>().ClosePanel();
        }

        base.OpenPanel();
    }

    private void OnClickBGM()
    {
        var isMute = App.Manager.Sound.ToggleMute(AudioType.BGM);
        SetSoundText(bgmTMP, isMute);
    }

    private void OnClickSFX()
    {
        var isMute = App.Manager.Sound.ToggleMute(AudioType.SFX);
        SetSoundText(sfxTMP, isMute);
    }

    private void SetSoundText(TextMeshProUGUI _tmp, bool _isMute)
    {
        _tmp.text = _isMute ? "OFF" : "ON";
    }
}