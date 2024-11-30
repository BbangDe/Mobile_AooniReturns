using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : UIBase
{
    [SerializeField] Button arrowBtn;
    [SerializeField] Button barrelBtn;

    private CharacterCtrl myCharCtrl;

    public override void Init()
    {
        arrowBtn.onClick.AddListener(OnClickArrow);
        barrelBtn.onClick.AddListener(OnClickBarrel);

        StartCoroutine(WaitForMyChar());
    }

    private IEnumerator WaitForMyChar()
    {
        yield return new WaitUntil(() => App.Manager.Player.MyCtrl != null);

        myCharCtrl = App.Manager.Player.MyCtrl;
    }

    private void OnClickArrow()
    {
        StartCoroutine(WaitForCoolTime());
        myCharCtrl.arrow.FireArrow();
    }

    private void OnClickBarrel()
    {

    }

    private IEnumerator WaitForCoolTime()
    {
        arrowBtn.enabled = false;
        arrowBtn.image.color = new Color(1, 1, 1, 0.5f);

        float time = 0;

        while (time <= 1)
        {
            time += Time.deltaTime;

            arrowBtn.image.fillAmount = time / 1f;

            yield return null;
        }

        arrowBtn.enabled = true;
        arrowBtn.image.color = Color.white;
    }

    private IEnumerator WaitForCoolTime2()
    {
        barrelBtn.enabled = false;
        barrelBtn.image.color = new Color(1, 1, 1, 0.5f);

        float time = 0;

        while (time <= 1)
        {
            time += Time.deltaTime;

            barrelBtn.image.fillAmount = time / 1f;

            yield return null;
        }

        barrelBtn.enabled = true;
        barrelBtn.image.color = Color.white;
    }
}
