using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimePanel : UIBase
{
    [SerializeField] TextMeshProUGUI timeTMP;

    private float intervalTime;

    private float remainTime;

    public float Remaining => remainTime;

    public override void Init()
    {

    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        remainTime = App.Manager.Game.GameTime;
        intervalTime = Time.time;

        UpdateText();
    }

    private void Update()
    {
        if (Time.time - intervalTime < 1f) return;

        intervalTime = Time.time;
        remainTime--;

        UpdateText();
    }

    private void UpdateText()
    {
        int min = Mathf.FloorToInt(remainTime / 60f);
        int sec = Mathf.FloorToInt(remainTime % 60f);

        timeTMP.text = string.Format("{0:0}:{1:00}", min, sec);
    }
}
