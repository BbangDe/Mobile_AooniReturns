using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class OniCtrl : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] CharacterUICtrl uiCtrl;

    public float CurrHP { get; private set; } = 100f;

    [Networked] public bool IsInvincible { get; private set; } = false;

    protected CharacterCtrl ownerCtrl;

    public override void Spawned()
    {
        ownerCtrl = transform.parent.GetComponent<CharacterCtrl>();
    }

    public void Setup()
    {
        CurrHP = 100f;
        uiCtrl.SetHP(CurrHP);

        IsInvincible = false;
        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!App.Manager.Game.IsGamePlay)
        {
            return;
        } 

        if (collision.CompareTag("Human"))
        {
            if (collision.transform.parent.TryGetComponent<CharacterCtrl>(out var charCtrl))
            {
                InteractHuman(charCtrl);
            }
        }
    }

    protected abstract void InteractHuman(CharacterCtrl _charCtrl);

    public void Attacked(float _damage)
    {
        RPC_Attacked(_damage);
    }

    [Rpc]
    private void RPC_Attacked(float _damage)
    {
        CurrHP -= _damage;
        uiCtrl.SetHP(CurrHP);

        if (CurrHP <= 0)
        {
            ownerCtrl.SetCharacterDead(true);
        }
        else
        {
            StartCoroutine(AttackedAnimation());
        }
    }

    private IEnumerator AttackedAnimation()
    {
        IsInvincible = true;
        sprite.color = Color.red;

        yield return new WaitForSeconds(1);

        IsInvincible = false;
        sprite.color = Color.white;
    }
}
