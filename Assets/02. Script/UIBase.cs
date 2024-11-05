using System;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public virtual Type GetPanelType() => GetType();

    public virtual UIState GetUIState() => UIState.Normal;

    /// <summary>
    /// Is Panel need to add ui stack?
    /// </summary>
    /// <returns></returns>
    public virtual bool IsAddUIStack() => false;

    /// <summary>
    /// Initialize Panel 
    /// called once on Awake, first time
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Open Panel (virtual, choose override)
    /// </summary>
    public virtual void OpenPanel()
    {
        if (IsAddUIStack() && !gameObject.activeSelf)
        {
            App.Manager.UI.AddUIStack(GetUIState());
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Close Panel (virtual, choose override)
    /// </summary>
    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);

        if (IsAddUIStack())
        {
            App.Manager.UI.PopUIStack(GetUIState());
        }
    }
}
