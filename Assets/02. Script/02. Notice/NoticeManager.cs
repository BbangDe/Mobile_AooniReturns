using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoBehaviour
{
    [SerializeField] Button continueBtn;
    [SerializeField] Button ratingBtn;

    private void Start()
    {
        continueBtn.onClick.AddListener(OnClickContinue);
        ratingBtn.onClick.AddListener(OnClickContinue);
    }

    private void OnClickContinue()
    {
        App.Manager.Network.JoinLobby();
    }

    private void OnClickRating()
    {

    }
}
