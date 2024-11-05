using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class JoinRoomBtn : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameTMP;
    [SerializeField] TextMeshProUGUI modeTMP;
    [SerializeField] TextMeshProUGUI playerCountTMP;

    SessionInfo currInfo;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClickJoinRoom);
    }

    public void SetInfo(SessionInfo _info)
    {
        gameObject.SetActive(true);

        currInfo = _info;

        roomNameTMP.text = _info.Name;
        modeTMP.text = GetModeName(_info.Properties["GameMode"]);
        playerCountTMP.text = string.Format("{0}/{1}", _info.PlayerCount, _info.MaxPlayers);
    }

    private string GetModeName(int _modeIndex) => (ModeType)_modeIndex switch
    {
        ModeType.Infection => "감염 모드",
        ModeType.Bomb => "폭탄 모드",
        ModeType.Police => "도둑과 경찰 모드",
        ModeType.Dual => "듀얼 모드",
        _ => "감염 모드"
    }; 

    public void SetNone()
    {
        gameObject.SetActive(false);

        currInfo = null;
    }

    private void OnClickJoinRoom()
    {
        App.Manager.Network.JoinMatch(currInfo);
    }
}
