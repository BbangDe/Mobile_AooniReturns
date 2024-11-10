using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class CharacterUICtrl : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI playerInfoTMP;
    [SerializeField] Image hpFillImg;

    public override void Spawned()
    {
        playerInfoTMP.text = $"Lv.{CalculateLevel(App.Data.Player.ExperiencePoints)} {App.Data.Player.NickName}";
    }

    public void SetHP(float _value)
    {
        hpFillImg.fillAmount = _value / 100f;
    }

    private int CalculateLevel(int _totalExp)
    {
        int currLevel = 1;
        int requiredExp = 50;

        while (_totalExp >= requiredExp)
        {
            _totalExp -= requiredExp;
            currLevel++;
            requiredExp += 100;
        }

        return currLevel;
    }


}
