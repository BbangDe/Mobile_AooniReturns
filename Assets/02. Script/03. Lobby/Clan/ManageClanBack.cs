using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageClanBack : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clanNameTMP;
    [SerializeField] TMP_InputField inviteNickInput;
    [SerializeField] Button inviteBtn;
    [SerializeField] Button deleteBtn;

    private void Awake()
    {
        inviteBtn.onClick.AddListener(OnClickInvite);
        deleteBtn.onClick.AddListener(OnClickDelete);
    }

    private void OnEnable()
    {
        inviteNickInput.text = string.Empty;
    }

    private void OnClickInvite()
    {
        inviteNickInput.text = string.Empty;
    }

    private void OnClickDelete()
    {
        App.Data.Clan.DeleteClan(
        ()=>
        {
            App.Manager.UI.GetPanel<ClanPanel>().CheckHasClan();
            App.Manager.UI.GetPanel<PlayerPanel>().SetPlayerClan();
        },
        (error) =>
        {
            Debug.LogError("Failed to get clan name: " + error);
        });
    }

    public void SetClanName()
    {
        App.Data.Clan.GetClanName(
        (clanName) =>
        {
            clanNameTMP.text = string.Format("클랜 <color=#00FF00>{0}</color>", clanName);
        },
        (error) =>
        {
            Debug.LogError("Failed to get clan name: " + error);
        });
    }
}
