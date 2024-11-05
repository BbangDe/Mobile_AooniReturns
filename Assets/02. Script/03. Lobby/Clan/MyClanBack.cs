using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyClanBack : MonoBehaviour
{
    [SerializeField] GameObject hasClanBack;
    [SerializeField] GameObject noClanBack;

    [Header("HasClanBack")]
    [SerializeField] TextMeshProUGUI clanNameTMP;
    [SerializeField] Button outBtn;

    [Header("NoClanBack")]
    [SerializeField] TMP_InputField createClanInput;
    [SerializeField] Button createBtn;

    private List<ClanMemberBack> memberBackList;

    private void Awake()
    {
        outBtn.onClick.AddListener(OnClickOut);
        createBtn.onClick.AddListener(OnClickCreate);

        memberBackList = GetComponentsInChildren<ClanMemberBack>(true).ToList();
    }

    private void OnClickOut()
    {
        App.Data.Clan.LeaveClan(()=>
        {
            App.Manager.UI.GetPanel<ClanPanel>().CheckHasClan();
            App.Manager.UI.GetPanel<PlayerPanel>().SetPlayerClan();
        }, null);
    }

    private void OnClickCreate()
    {
        if (string.IsNullOrEmpty(createClanInput.text)) return;

        App.Data.Clan.CreateClan(createClanInput.text,
        (result) =>
        {
            App.Manager.UI.GetPanel<ClanPanel>().CheckHasClan();
            App.Manager.UI.GetPanel<PlayerPanel>().SetPlayerClan();
        }, null);
    }

    public void SetActiveToHasClan(bool _hasClan)
    {
        hasClanBack.SetActive(_hasClan);
        noClanBack.SetActive(!_hasClan);

        if (_hasClan)
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

            App.Data.Clan.GetClanMembers(
            (result) =>
            {
                int index = 0;

                foreach (var role in result)
                {
                    foreach (var member in role.Members)
                    {
                        memberBackList[index++].Init(member.Key.Id);
                    }
                }
            },
            (error) =>
            {
                Debug.LogError("Failed to get clan name: " + error);
            });
        }
    }

    public void SetActiveToIsClanOwner(bool _isOwner)
    {
        outBtn.gameObject.SetActive(!_isOwner);
    }
}
