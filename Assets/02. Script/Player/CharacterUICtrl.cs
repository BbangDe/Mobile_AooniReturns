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

    private const string infoString = "Lv.{0} {1}";

    public override void Spawned()
    {
        playerInfoTMP.text = string.Format(infoString, 1, 2);
    }

    public void SetHP(float _value)
    {
        hpFillImg.fillAmount = _value / 100f;
    }
}
