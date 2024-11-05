using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class OniBombCtrl : OniCtrl
{
    LayerMask mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("Map");
    }

    protected override void InteractHuman(CharacterCtrl _charCtrl)
    {
        RPC_GiveBomb(_charCtrl, GetPosition());
    }

    [Rpc]
    private void RPC_GiveBomb(CharacterCtrl _charCtrl, Vector2 _randomPosition)
    {
        ownerCtrl.SetCharacterState(0);

        _charCtrl.MoveToRandomPosition(_randomPosition);
        _charCtrl.SetCharacterState(1);
    }

    private Vector2 GetPosition()
    {
        Vector2 randomPosition;
        int attempts = 0;

        do
        {
            randomPosition = GetRandomPosition();
            attempts++;
        }
        while (IsPositionColliding(randomPosition) && attempts < 1000);

        if (attempts >= 1000)
        {
            Debug.LogWarning("Could not find a valid random position after " + 1000 + " attempts.");
        }
        else
        {
            return randomPosition;
        }

        return Vector2.zero;
    }

    private Vector2 GetRandomPosition()
    {
        float randomX = Random.Range(-9.4f, 53.2f);
        float randomY = Random.Range(-57.6f, 1.3f);

        return new Vector2(randomX, randomY);
    }

    private bool IsPositionColliding(Vector2 position)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.1f, mask);
        return hitCollider != null;
    }
}
