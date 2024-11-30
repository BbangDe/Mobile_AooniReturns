using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : UIManager
{
    [SerializeField] Button optionBtn;
    [SerializeField] Button shopBtn;
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button cafeBtn;
    [SerializeField] Button rankBtn;
    [SerializeField] Button clanBtn;

    [SerializeField] GameObject alertPanel;
    [SerializeField] TMP_Text alertTxt;

    protected override void Start()
    {
        base.Start();

        cafeBtn.onClick.AddListener(OnClickCafe);

        optionBtn.onClick.AddListener(() => GetPanel<OptionPanel>().OpenPanel());
        shopBtn.onClick.AddListener(() => GetPanel<ShopPanel>().OpenPanel());
        //rankBtn.onClick.AddListener(() => GetPanel<RankPanel>().OpenPanel());
        //clanBtn.onClick.AddListener(() => GetPanel<ClanPanel>().OpenPanel());

        rankBtn.onClick.AddListener(NowBuilding);
        clanBtn.onClick.AddListener(NowBuilding);

        createRoomBtn.onClick.AddListener(() =>
        {
            GetPanel<CreateRoomPanel>().OpenPanel();
            GetPanel<JoinRoomPanel>().ClosePanel();
        });
    }

    private void OnClickCafe()
    {
        Application.OpenURL("https://m.cafe.naver.com/onireturns");
    }

    private void NowBuilding()
    {
        alertTxt.text = "아직 개발중입니다";
        alertPanel.SetActive(true);
    }
}
