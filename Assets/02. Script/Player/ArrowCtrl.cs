using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using DG.Tweening;

public class ArrowCtrl : NetworkBehaviour
{
    [SerializeField] Animator animCtrl;

    private Projectiles selected_projectile;
    private Vector3 startPosition = new(0, 0.7f, 0);
    private CharacterCtrl myCharCtrl;

    private void Start()
    {
        gameObject.SetActive(false);
        myCharCtrl = GetComponentInParent<CharacterCtrl>();

        RPC_SelectedProjectile();
    }

    public void FireArrow()
    {
        RPC_FireArrow(myCharCtrl.LastChangeDir);
    }

    [Rpc]
    private void RPC_FireArrow(Vector2 _dir)
    {
        App.Manager.Sound.PlaySFX("SFX_Skill_Bow");
        gameObject.SetActive(true);

        Vector2 normalizedDir = NormalizeDirection(_dir);
        Vector3 targetPosition = transform.position + new Vector3(normalizedDir.x, normalizedDir.y, 0) * Projectile.projectile.distance;

        animCtrl.SetFloat("MoveX", normalizedDir.x);
        animCtrl.SetFloat("MoveY", normalizedDir.y);

        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / Projectile.projectile.speed;

        transform.DOMove(targetPosition, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localPosition = startPosition;
        });
    }

    [Rpc]
    void RPC_SelectedProjectile()
    {
        NetworkObject playerObject = Runner.GetPlayerObject(Runner.LocalPlayer);
        string playerName = playerObject != null ? playerObject.name : "Unknown Player";


        switch (selected_projectile)
        {
            case 0:
                Projectile.projectile.shooter_name = playerName;
                Projectile.projectile.distance = 2;
                Projectile.projectile.speed = 1.1f;
                break;
        }
    }

    private Vector2 NormalizeDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return new Vector2(Mathf.Sign(dir.x), 0);
        }
        else
        {
            return new Vector2(0, Mathf.Sign(dir.y));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!App.Manager.Game.IsGamePlay)
        {
            //return;
        }

        if (collision.CompareTag("Oni"))
        {
            if (collision.TryGetComponent<OniCtrl>(out var oniCtrl))
            {/*
                if (!oniCtrl.IsInvincible)
                {
                    App.Manager.Sound.PlaySFX("SFX_Game_Slash");

                    transform.DOKill();

                    gameObject.SetActive(false);
                    transform.localPosition = startPosition;

                    oniCtrl.Attacked(10);
                }*/
                Debug.Log($"{gameObject.name} 활 트리거 들어옴");
                App.Manager.Sound.PlaySFX("SFX_Game_Slash");
            }
        }
    }

}
