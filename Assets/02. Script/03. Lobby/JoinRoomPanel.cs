using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomPanel : UIBase
{
    [SerializeField] Transform roomListParent;
    private JoinRoomBtn[] roomBtns;

    public override UIState GetUIState() => UIState.JoinRoom;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        roomBtns = roomListParent.GetComponentsInChildren<JoinRoomBtn>(true);

        OpenPanel();
    }

    public void SetRoomList(List<SessionInfo> _sessionList)
    {
        int i = 0;

        Debug.Log($"접속가능방 수 : {_sessionList.Count}");
        for (; i < _sessionList.Count; i++)
        {
            roomBtns[i].SetInfo(_sessionList[i]);
        }

        for (; i < roomListParent.childCount; i++)
        {
            roomBtns[i].SetNone();
        }
    }
}
