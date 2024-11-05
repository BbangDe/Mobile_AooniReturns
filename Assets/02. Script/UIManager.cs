using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum UIState
{
    Normal,
    SignUp,
    CreateRoom,
    JoinRoom,
    Clan,
    Shop,
    Rank,
    Option,
}

public class UIManager : Fusion.Behaviour
{
    [HideInInspector]
    public UIState CurrState
        => UIStack.Count == 0 ? UIState.Normal : UIStack.Peek();

    private Dictionary<Type, UIBase> UIDic;
    private Stack<UIState> UIStack;

    protected virtual void Awake()
    {
        ManagerBase.SetFieldValue(typeof(UIManager), this);

        var UIs = GetComponentsInChildren<UIBase>(true);

        UIDic = new(UIs.Length);
        UIStack = new();

        foreach (var UI in UIs)
        {
            UIDic.Add(UI.GetPanelType(), UI);
        }
    }

    protected virtual void Start()
    {
        InitUIs();
    }

    private void InitUIs()
    {
        foreach (var UI in UIDic.Values)
        {
            if (!UI.gameObject.activeSelf) //wake up panels
            {
                UI.gameObject.SetActive(true);
                UI.gameObject.SetActive(false);
            }

            try { UI.Init(); }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }

    #region Get Panel
    public T GetPanel<T>() where T : UIBase => (T)UIDic[typeof(T)];

    public bool TryGetPanel<T>(out T _panel) where T : UIBase
    {
        if (UIDic.TryGetValue(typeof(T), out var panel))
        {
            _panel = (T)panel;
            return true;
        }

        _panel = default;
        return false;
    }
    #endregion

    #region UI Stack Managing
    public void AddUIStack(UIState _state)
    {
        UIStack.Push(_state);
    }

    public void PopUIStack(UIState _state = 0)
    {
        if (CurrState != _state) return;

        UIStack.Pop();
    }
    #endregion
}
