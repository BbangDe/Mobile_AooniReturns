using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class PlayerManager : NetManager
{
    public List<NetworkObject> networkObjList = new(8);
    public List<CharacterCtrl> characterList = new(8);

    public CharacterCtrl MyCtrl { get; private set; }

    public IReadOnlyList<CharacterCtrl> AllPlayers => characterList;

    public IReadOnlyList<CharacterCtrl> HumanPlayers => characterList.Where(x => x.CurrState == CharacterType.Human).ToList();
    public IReadOnlyList<CharacterCtrl> OniPlayers => characterList.Where(x => x.CurrState == CharacterType.Oni).ToList();

    public void SubmitPlayer(CharacterCtrl _char)
    {
        if (characterList.Contains(_char))
        {
            return;
        }

        characterList.Add(_char);
        networkObjList.Add(_char.Object);

        if (_char.Object.StateAuthority.PlayerId == App.Manager.Network.Runner.LocalPlayer.PlayerId)
        {
            MyCtrl = _char;
        }
    }

    public void SetRandomOni()
    {
        var randomIndex = Random.Range(0, AllPlayers.Count);

        SetOni(networkObjList[randomIndex].Id);
        RPC_SetOni(networkObjList[randomIndex].Id);
    }

    public void SetOni(NetworkId characterNetworkId)
    {
        for (int i = 0; i < networkObjList.Count; i++)
        {
            if (networkObjList[i].Id == characterNetworkId)
            {
                characterList[i].SetCharacterState(1);
                App.Manager.UI.GetPanel<NoticePanel>().NoticeBecomeOni();
            }
        }
    }

    [Rpc]
    private void RPC_SetOni(NetworkId characterNetworkId)
    {
        for (int i = 0; i < networkObjList.Count; i++)
        {
            if (networkObjList[i].Id == characterNetworkId)
            {
                characterList[i].SetCharacterState(1);
                App.Manager.UI.GetPanel<NoticePanel>().NoticeBecomeOni();
            }
        }
    }

    public void SetAllHuman()
    {
        RPC_SetHuman();

        foreach (var character in characterList)
        {
            character.SetCharacterState(0);
        }
    }

    [Rpc]
    private void RPC_SetHuman()
    {
        foreach (var character in characterList)
        {
            character.SetCharacterState(0);
        }
    }

    [Rpc]
    private void RPC_SetBucket(NetworkId characterNetworkId)
    {
        for (int i = 0; i < networkObjList.Count; i++)
        {
            if (networkObjList[i].Id == characterNetworkId)
            {
                if (characterList[i].CurrState.Equals(CharacterType.Human))
                {
                    characterList[i].SetCharacterState(2);
                }
                else if (characterList[i].CurrState.Equals(CharacterType.Bucket))
                {
                    characterList[i].SetCharacterState(0);
                }
            }
        }
    }

    public void SetBucket(NetworkId characterNetworkId)
    {
        for (int i = 0; i < networkObjList.Count; i++)
        {
            if (networkObjList[i].Id == characterNetworkId)
            {
                if(characterList[i].CurrState.Equals(CharacterType.Human))
                {
                    characterList[i].SetCharacterState(2);
                }
                else if (characterList[i].CurrState.Equals(CharacterType.Bucket))
                {
                    characterList[i].SetCharacterState(0);
                }
            }
        }

        RPC_SetBucket(characterNetworkId);
    }
}