using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class OniInfectionCtrl : OniCtrl
{
    protected override void InteractHuman(CharacterCtrl _charCtrl)
    {
        RPC_SetHumanToOni(_charCtrl);
    }

    [Rpc]
    private void RPC_SetHumanToOni(CharacterCtrl _charCtrl)
    {
        _charCtrl.SetCharacterState(1);
    }
}
