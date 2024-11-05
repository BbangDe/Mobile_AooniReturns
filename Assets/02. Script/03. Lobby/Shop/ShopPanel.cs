using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanel : UIBase
{
    [Header("Buttons")]
    [SerializeField] Button oniBtn;
    [SerializeField] Button humanBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button buyBtn;

    [Header("ScrollRect")]
    [SerializeField] ScrollRect oniScrollBack;
    [SerializeField] ScrollRect humanScrollBack;

    [Header("TextMeshProUGUI")]
    [SerializeField] TextMeshProUGUI itemNameTMP;
    [SerializeField] TextMeshProUGUI goldTMP;

    private ShopItemBtn[] oniItemBtns;
    private ShopItemBtn[] humanItemBtns;

    public override UIState GetUIState() => UIState.Shop;

    public override bool IsAddUIStack() => true;

    public override void Init()
    {
        oniBtn.onClick.AddListener(OnClickOni);
        humanBtn.onClick.AddListener(OnClickHuman);
        backBtn.onClick.AddListener(ClosePanel);
        buyBtn.onClick.AddListener(OnClickBuy);

        oniItemBtns = oniScrollBack.GetComponentsInChildren<ShopItemBtn>();
        humanItemBtns = humanScrollBack.GetComponentsInChildren<ShopItemBtn>();

        InitBtns();
    }

    public void InitBtns()
    {
        var shopData = App.Data.Title.shopDatas;

        var oniData = shopData.Where(x => x.Value.type == 1).Select(x => x.Value).ToList();
        var humanData = shopData.Where(x => x.Value.type == 2).Select(x => x.Value).ToList();

        for (int i = 0; i < oniItemBtns.Length; i++) 
        {
            if (i >= oniData.Count)
            {
                oniItemBtns[i].SetNone();
                continue;
            }

            oniItemBtns[i].SetItem(oniData[i]);
        }

        for (int i = 0; i < humanItemBtns.Length; i++)
        {
            if (i >= humanData.Count)
            {
                humanItemBtns[i].SetNone();
                continue;
            }

            humanItemBtns[i].SetItem(humanData[i]);
        }
    }

    public override void OpenPanel()
    {
        if (App.UI.Lobby.CurrState == UIState.CreateRoom)
        {
            App.UI.Lobby.GetPanel<CreateRoomPanel>().ClosePanel();
        }

        base.OpenPanel();

        OnClickOni();
        goldTMP.text = App.Data.Player.Currency.ToString();
    }

    private void OnClickOni()
    {
        oniScrollBack.gameObject.SetActive(true);
        humanScrollBack.gameObject.SetActive(false);

        oniScrollBack.horizontalNormalizedPosition = 0f;

        ResetSelectedItem();
        ResetUsingItem();

        itemNameTMP.text = string.Empty;
    }

    private void OnClickHuman()
    {
        oniScrollBack.gameObject.SetActive(false);
        humanScrollBack.gameObject.SetActive(true);

        humanScrollBack.horizontalNormalizedPosition = 0f;

        ResetSelectedItem();
        ResetUsingItem();

        itemNameTMP.text = string.Empty;
    }

    private void OnClickBuy()
    {
        ShopItemBtn selectedBtn;

        if (oniScrollBack.gameObject.activeSelf)
        {
            selectedBtn = oniItemBtns.FirstOrDefault(x => x.IsSelected);
        }
        else
        {
            selectedBtn = humanItemBtns.FirstOrDefault(x => x.IsSelected);
        }

        if (selectedBtn == null) return;

        if (selectedBtn.Cost > App.Data.Player.Currency)
        {
            //warning
            return;
        }
        else
        {
            int index;

            if (oniScrollBack.gameObject.activeSelf)
            {
                index = Array.IndexOf(oniItemBtns, selectedBtn);
                App.Data.Player.SetOniSkinIndex(index, null, null);
            }
            else
            {
                index = Array.IndexOf(humanItemBtns, selectedBtn);
                App.Data.Player.SetHumanSkinIndex(index, null, null);
            }

            var newCurrency = App.Data.Player.Currency - selectedBtn.Cost;
            App.Data.Player.SetCurrency(newCurrency, null, null);

            App.Manager.UI.GetPanel<PlayerPanel>().SetPlayerSkin();
        }

        ResetSelectedItem();
        ResetUsingItem();
    }

    public void OnClickItem(ShopItemBtn _targetItem)
    {
        ResetSelectedItem();

        _targetItem.SetSelected(true);
        itemNameTMP.text = _targetItem.Name;
    }

    public void ResetSelectedItem()
    {
        if (oniScrollBack.gameObject.activeSelf)
        {
            foreach (var btn in oniItemBtns)
            {
                btn.SetSelected(false);
            }
        }
        else
        {
            foreach (var btn in humanItemBtns)
            {
                btn.SetSelected(false);
            }
        }
    }

    public void ResetUsingItem()
    {
        if (oniScrollBack.gameObject.activeSelf)
        {
            foreach (var btn in oniItemBtns)
            {
                btn.SetUsing(false);
            }
        }
        else
        {
            foreach (var btn in humanItemBtns)
            {
                btn.SetUsing(false);
            }
        }

        oniItemBtns[App.Data.Player.OniSkinIndex].SetUsing(true);
        humanItemBtns[App.Data.Player.HumanSkinIndex].SetUsing(true);
    }
}
