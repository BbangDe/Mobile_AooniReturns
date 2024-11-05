using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ManagerBase
{
    protected static IEnumerable<FieldInfo> AppFieldInfo;

    static ManagerBase()
    {
        var flag = BindingFlags.Instance | BindingFlags.NonPublic;
        AppFieldInfo = typeof(App).GetFields(flag);
    }

    internal static FieldInfo SetFieldValue(Type type, MonoBehaviour manager)
    {
        var fields = AppFieldInfo.Where(field => field.FieldType.IsAssignableFrom(type));
        if (fields == null || fields.Count() != 1)
        {
            Debug.LogError($"Unresolved manager found. Type: {type.Name}");
            return null;
        }

        var targetField = fields.ElementAt(0);
        targetField.SetValue(App.instance, manager);
        return targetField;
    }

    internal static FieldInfo SetFieldValue(MonoBehaviour manager)
    {
        return SetFieldValue(manager.GetType(), manager);
    }
}

/// <summary>
/// Base manager class.
/// If the manager class use network functionality, please use GameManager.
/// By calling Awake function, manager will be registered to App(manager router).
/// Manager will not be unregistered on destroy. 
/// Instead it will be overriden on new manager (of same type) appears.
/// </summary>
public class Manager : Fusion.Behaviour
{
    private FieldInfo fieldInfo;

    protected virtual void Awake()
    {
        fieldInfo = ManagerBase.SetFieldValue(this);
    }

    protected virtual void OnDestroy()
    {
        if (fieldInfo == null)
        {
            return;
        }

        fieldInfo.SetValue(App.instance, null);
    }
}

public class SimManager : Fusion.SimulationBehaviour
{
    private FieldInfo fieldInfo;

    protected virtual void Awake()
    {
        fieldInfo = ManagerBase.SetFieldValue(this);
        App.Manager.Network.Runner.AddGlobal(this);
    }

    protected virtual void OnDestroy()
    {
        if (fieldInfo == null)
        {
            return;
        }

        fieldInfo.SetValue(App.instance, null);
    }
}

public class ViewManager : Fusion.Behaviour
{
    private FieldInfo fieldInfo;

    protected virtual void Awake()
    {
        fieldInfo = ManagerBase.SetFieldValue(this);
    }

    protected virtual void OnDestroy()
    {
        if (fieldInfo == null)
        {
            return;
        }

        fieldInfo.SetValue(App.instance, null);
    }
}

public class NetManager : Fusion.NetworkBehaviour
{
    private FieldInfo fieldInfo;

    protected virtual void Awake()
    {
        fieldInfo = ManagerBase.SetFieldValue(this);
    }

    protected virtual void OnDestroy()
    {
        if (fieldInfo == null)
        {
            return;
        }

        fieldInfo.SetValue(App.instance, null);
    }
}