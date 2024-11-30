using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NoticePanel : UIBase
{
    [SerializeField] TextMeshProUGUI noticeTMP;
    private RectTransform noticeRect;

    private Sequence sequence;
    private IEnumerator tmpAnim;

    private const string beforeGameStart = "잠시 뒤에 숙주 오니가 결정됩니다.\n서로 멀리 떨어지세요!";
    private const string countDown = "{0}초 남았습니다!";
    private const string becomeOni = "{0}님이 숙주오니가 되었습니다!";

    public override void Init()
    {
        sequence = DOTween.Sequence();

        noticeRect = noticeTMP.GetComponent<RectTransform>();

        noticeTMP.text = string.Empty;
    }

    private void ShowTMP()
    {
        if (tmpAnim != null)
        {
            StopCoroutine(tmpAnim);
        }

        tmpAnim = PlayTMPAnim();
        StartCoroutine(tmpAnim);
    }

    private IEnumerator PlayTMPAnim()
    {
        //if (sequence != null)
        //{
        //    sequence.Kill();
        //}

        //sequence = DOTween.Sequence();

        //sequence.Append(noticeRect.DOScale(Vector3.zero, 0f))
        //    .Append(noticeRect.DOScale(Vector3.one, 0.25f))
        //    .AppendInterval(1f)
        //    .Append(noticeRect.DOScale(Vector3.zero, 0.25f));

        Debug.Log("여기 니 그럼");

        noticeRect.DOKill();

        //noticeRect.transform.localScale = Vector3.zero;
        noticeRect.transform.localScale = Vector3.one;
        //noticeRect.DOScale(Vector3.one, 0.25f);

        yield return new WaitForSeconds(1.25f);

        //noticeRect.DOScale(Vector3.zero, 0.25f);
        noticeRect.transform.localScale = Vector3.zero;
    }

    public void NoticeBeforeGameStart()
    {
        base.OpenPanel();

        noticeTMP.text = beforeGameStart;
        ShowTMP();
        //PlayTMPAnim();
    }

    public void NoticeCountDown()
    {
        base.OpenPanel();

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        int time = 10;
        while (time > 0)
        {
            noticeTMP.text = string.Format(countDown, time--);
            ShowTMP();
            //PlayTMPAnim();

            yield return new WaitForSeconds(1);
        }

        noticeTMP.text = string.Empty;
    }

    public void NoticeBecomeOni()
    {
        base.OpenPanel();

        noticeTMP.text = string.Format(becomeOni, 1);
        ShowTMP();
        //PlayTMPAnim();
    }
}
