using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPanel : UIBase
{
    [SerializeField] TextMeshProUGUI nickTMP;

    [SerializeField] TextMeshProUGUI recordTMP;
    [SerializeField] TextMeshProUGUI clanTMP;
    [SerializeField] TextMeshProUGUI rankTMP;
    [SerializeField] TextMeshProUGUI goldTMP;

    [SerializeField] TextMeshProUGUI levelTMP;
    [SerializeField] TextMeshProUGUI expTMP;
    [SerializeField] Image expImg;

    [SerializeField] Animator humanAnim;
    [SerializeField] Animator oniAnim;

    public override void Init()
    {
        nickTMP.text = App.Data.Player.NickName;
        recordTMP.text = string.Format("{0}오니 / {1}히로시 / {2}생존", App.Data.Player.OniKills, App.Data.Player.HiroshiKills, App.Data.Player.SurvivalCount);

        goldTMP.text = App.Data.Player.Currency.ToString();

        SetPlayerLevel();
        SetPlayerClan();
        SetPlayerRank();

        SetPlayerSkin();
    }

    private void SetPlayerLevel()
    {
        var result = CalculateLevel(App.Data.Player.ExperiencePoints);

        levelTMP.text = string.Format("Lv. {0}", result.level.ToString());

        var percentage = result.remainingExp / (float)result.requiredExp * 100;
        expTMP.text = string.Format("{0}/{1} ({2}%)", result.remainingExp, result.requiredExp, Mathf.Round(percentage * 100f) / 100f);

        expImg.fillAmount = result.remainingExp / (float)result.requiredExp;
    }

    private (int level, int remainingExp, int requiredExp) CalculateLevel(int _totalExp)
    {
        int currLevel = 1; 
        int requiredExp = 50; 

        while (_totalExp >= requiredExp)
        {
            _totalExp -= requiredExp;
            currLevel++;
            requiredExp += 100; 
        }

        return (currLevel, _totalExp, requiredExp);
    }
    
    public void SetPlayerClan()
    {
        if (string.IsNullOrEmpty(App.Data.Player.Clan))
        {
            clanTMP.gameObject.SetActive(false);
        }
        else
        {
            clanTMP.gameObject.SetActive(true);

            App.Data.Clan.GetClanName(
            (result) =>
            {
                clanTMP.text = string.Format("클랜 <color=#00FF00>{0}</color>", result);
            }, null);
        }
    }

    public void SetPlayerRank()
    {
        App.Data.Player.GetLeaderboardRank(App.Data.Player.PlayerID, 
        (rank, isRanked) =>
        {
            var rankText = isRanked ? rank.ToString() : "";
            rankTMP.text = string.Format("전체 랭킹 <color=#00FF00>{0}</color>", rankText);
        },
        (error)=>
        {
            rankTMP.text = "전체 랭킹 <color=#00FF00>순위권 외</color>";
        });
    }

    public void SetPlayerSkin()
    {
        humanAnim.SetTrigger(App.Data.Player.HumanSkinIndex.ToString());
        oniAnim.SetTrigger(App.Data.Player.OniSkinIndex.ToString());
    }
}
