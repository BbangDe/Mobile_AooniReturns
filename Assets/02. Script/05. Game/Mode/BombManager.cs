using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : GameManager
{
    protected override void Awake()
    {
        base.Awake();

        MaxRoundCount = 3;
        GameTime = 60;

        for(int i=0; i < App.Manager.Player.characterList.Count; i++)
        {
            Debug.Log($"{App.Manager.Player.characterList[i].name}");
        }
    }

    protected override bool CheckVictoryCondition()
    {
        if (App.Manager.UI.GetPanel<TimePanel>().Remaining <= 0f)
        {
            return true;
        }

        return false;
    }
}
