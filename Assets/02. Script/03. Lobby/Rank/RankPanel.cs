using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankPanel : UIBase
{
    [SerializeField] Button[] rankBtns;
    [SerializeField] RankDetailBack[] rankDetailBacks;
    [SerializeField] Button backBtn;

    public override UIState GetUIState() => UIState.Rank;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        backBtn.onClick.AddListener(ClosePanel);

        for (int i = 0; i < rankBtns.Length; i++)
        {
            int index = i;

            rankBtns[index].onClick.AddListener(() => OnClickRank(index));
            rankDetailBacks[index].Init();
        }
    }

    public override void OpenPanel()
    {
        if (App.UI.Lobby.CurrState == UIState.CreateRoom)
        {
            App.UI.Lobby.GetPanel<CreateRoomPanel>().ClosePanel();
        }

        base.OpenPanel();

        OnClickRank(0);
    }

    private void OnClickRank(int _index)
    {
        for (int i = 0; i < rankDetailBacks.Length; i++)
        {
            if (i == _index)
            {
                rankDetailBacks[i].gameObject.SetActive(true);
            }
            else
            {
                rankDetailBacks[i].gameObject.SetActive(false);
            }
        }
    }
}
