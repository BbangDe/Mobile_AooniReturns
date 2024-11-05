using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemBtn : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI goldTMP;
    [SerializeField] GameObject usingTMP;
    [SerializeField] GameObject selectedImg;
    [SerializeField] Button selectedBtn;

    private ShopData data;

    public bool IsSelected { get; private set; }
    public bool IsUsing { get; private set; }

    public string Name => data.name;
    public int Cost => data.cost;

    private void OnEnable()
    {
        if (data != null)
        {
            anim.SetBool(data.resource, true);
        }
    }

    public void SetNone()
    {
        gameObject.SetActive(false);

        data = null;

        selectedBtn.onClick.RemoveAllListeners();
    }

    public void SetItem(ShopData _data)
    {
        gameObject.SetActive(true);

        data = _data;

        goldTMP.text = _data.cost.ToString();

        selectedBtn.onClick.AddListener(OnClickSelected);
    }

    private void OnClickSelected()
    {
        App.UI.Lobby.GetPanel<ShopPanel>().OnClickItem(this);
    }

    public void SetSelected(bool _isSelected)
    {
        selectedImg.SetActive(_isSelected);
        IsSelected = _isSelected;
    }

    public void SetUsing(bool _isUsing)
    {
        usingTMP.SetActive(_isUsing);
        IsUsing = _isUsing;
    }
}
