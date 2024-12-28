using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ReadyUIManager : UIManager
{
    [SerializeField] Button startBtn;
    [SerializeField] Button exitBtn;

    [SerializeField] TextMeshProUGUI roomNameTMP;
    [SerializeField] TextMeshProUGUI modeTMP;
    [SerializeField] TextMeshProUGUI playerCountTMP;

    [SerializeField] GameObject alertPanel;
    [SerializeField] TMP_Text alertText;


    protected override void Start()
    {
        base.Start();

        startBtn.onClick.AddListener(OnClickStart);
        exitBtn.onClick.AddListener(OnClickExit);

        SetRoomText();
        SetPlayerCount();

        StartCoroutine(WaitForMyChar());
    }

    private IEnumerator WaitForMyChar()
    {
        yield return new WaitUntil(() => App.Manager.Player.MyCtrl != null);

        startBtn.gameObject.SetActive(App.Manager.Network.Runner.IsSharedModeMasterClient);
    }

    private void OnClickStart()
    {
        var currSession = App.Manager.Network.Runner.SessionInfo;

        if(currSession.PlayerCount > 1)
        {
            App.Manager.Network.StartGame();
        }
        else
        {
            //alertText.text = "2명 이상이어야 플레이 가능합니다";
            //alertPanel.SetActive(true);
            App.Manager.Network.StartGame();
        }
        
    }

    private void OnClickExit()
    {
        App.Manager.Network.LeaveMatch();
    }

    private void SetRoomText()
    {
        var currSession = App.Manager.Network.Runner.SessionInfo;

        roomNameTMP.text = string.Format("[대기중] {0}", currSession.Name);

        var modeType = App.Manager.Network.Runner.SessionInfo.Properties["GameMode"];
        modeTMP.text = GetModeName(modeType);
    }

    private string GetModeName(int _modeIndex) => (ModeType)_modeIndex switch
    {
        ModeType.Infection => "감염",
        ModeType.Bomb => "폭탄",
        ModeType.Police => "도둑과 경찰",
        ModeType.Dual => "듀얼",
        _ => "감염"
    };

    public void SetPlayerCount()
    {
        var currSession = App.Manager.Network.Runner.SessionInfo;

        playerCountTMP.text = string.Format("{0}/{1}", currSession.PlayerCount, currSession.MaxPlayers);
    }
}
