using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomPanel : UIBase
{
    [SerializeField] Transform roomListParent;
    private JoinRoomBtn[] roomBtns;

    public List<SessionInfo> sessionList;

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

        sessionList = _sessionList;

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
