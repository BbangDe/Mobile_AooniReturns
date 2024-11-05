using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class OniDualCtrl : OniCtrl
{
    protected override void InteractHuman(CharacterCtrl _charCtrl)
    {
        RPC_TeleportHuman(_charCtrl);
    }

    [Rpc]
    private void RPC_TeleportHuman(CharacterCtrl _charCtrl)
    {
        //_charCtrl.MoveToRandomPosition();
    }
}
