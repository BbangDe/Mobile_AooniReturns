using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClanPanel : UIBase
{
    [SerializeField] Button backBtn;
    [SerializeField] Button myClanBtn;
    [SerializeField] Button manageClanBtn;

    [SerializeField] MyClanBack myClanBack;
    [SerializeField] ManageClanBack manageClanBack;

    public override UIState GetUIState() => UIState.Clan;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        backBtn.onClick.AddListener(ClosePanel);
        myClanBtn.onClick.AddListener(OnClickMyClan);
        manageClanBtn.onClick.AddListener(OnClickManageClan);
    }

    public override void OpenPanel()
    {
        if (App.UI.Lobby.CurrState == UIState.CreateRoom)
        {
            App.UI.Lobby.GetPanel<CreateRoomPanel>().ClosePanel();
        }

        CheckHasClan();
        OnClickMyClan();

        base.OpenPanel();
    }

    public void CheckHasClan()
    {
        var hasClan = !string.IsNullOrEmpty(App.Data.Player.Clan);;

        myClanBack.SetActiveToHasClan(hasClan);

        App.Data.Clan.CheckPlayerIsClanOwner(
            (result) =>
            {
                myClanBack.SetActiveToIsClanOwner(result);

                manageClanBack.SetClanName();
                manageClanBtn.gameObject.SetActive(result);
            }, null);
    }

    private void OnClickMyClan()
    {
        myClanBack.gameObject.SetActive(true);
        manageClanBack.gameObject.SetActive(false);
    }

    private void OnClickManageClan()
    {
        myClanBack.gameObject.SetActive(false);
        manageClanBack.gameObject.SetActive(true);
    }
}
