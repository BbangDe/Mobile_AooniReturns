using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DeveloperManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI progressTMP;
    [SerializeField] GameObject dataBack;
    [SerializeField] float delayTime = 3f;

    private void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => 0f, x => UpdateText(x), 100f, 1))
            .AppendInterval(1)
            .OnComplete(() => StartCoroutine(StartLoading()));
    }

    private void UpdateText(float value)
    {
        int roundedValue = Mathf.FloorToInt(value / 5f) * 5;
        progressTMP.text = roundedValue + "%";
    }

    private IEnumerator StartLoading()
    {
        dataBack.SetActive(false);

        yield return new WaitForSeconds(delayTime);

        App.LoadScene(SceneName.Title);
    }
}
