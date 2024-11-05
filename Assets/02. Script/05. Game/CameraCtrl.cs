using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();

        StartCoroutine(WaitForMyChatCtrl());
    }

    private IEnumerator WaitForMyChatCtrl()
    {
        yield return new WaitUntil(() => App.Manager.Player.MyCtrl != null);

        cam.Follow = App.Manager.Player.MyCtrl.transform;
    }
}
