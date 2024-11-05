using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : GameManager
{
    protected override void Awake()
    {
        base.Awake();

        MaxRoundCount = 8;
        GameTime = 120;
    }

    protected override bool CheckVictoryCondition()
    {
        if (App.Manager.Player.OniPlayers.Count == App.Manager.Player.AllPlayers.Count)
        {
            return true;
        }

        if (GetOniAllDead())
        {
            return true;
        }

        if (App.Manager.UI.GetPanel<TimePanel>().Remaining <= 0f)
        {
            return true;
        }

        return false;
    }

    private bool GetOniAllDead()
    {
        if (App.Manager.Player.OniPlayers.Count == 0)
        {
            return false;
        }

        foreach (var charCtrl in App.Manager.Player.OniPlayers)
        {
            if (charCtrl.Dead == true)
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
