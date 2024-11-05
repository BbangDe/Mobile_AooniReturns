using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public void EMP()
    {
        App.Data.Player.SetPlayerStatistics(70, 1, 2, 3, null, null);
    }
}
