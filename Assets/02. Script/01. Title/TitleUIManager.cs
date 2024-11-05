using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : UIManager
{
    [SerializeField] Button cafeBtn;

    protected override void Start()
    {
        base.Start();

        cafeBtn.onClick.AddListener(OnClickCafe);
    }

    private void OnClickCafe()
    {
        Application.OpenURL("https://m.cafe.naver.com/onireturns");
    }
}
