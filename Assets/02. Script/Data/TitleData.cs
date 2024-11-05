using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TitleData : Manager
{
    public Dictionary<string, ShopData> shopDatas = new();

    public bool isTitleDataLoaded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void LoadTitleData(Action _loadDataCallback, Action<PlayFabError> _errorHandler)
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (isTitleDataLoaded)
                {
                    return;
                }

                shopDatas.Clear();

                var charData = result.Data["ShopData"];
                var charDataList = JsonUtilityHelper.FromJson<ShopData>(charData);
                foreach (var data in charDataList)
                {
                    shopDatas.Add(data.code, data);
                }

                isTitleDataLoaded = true;
                _loadDataCallback();
            },
            _errorHandler);
    }
}

public class JsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "";
        if (json[0] == '{')
        {
            newJson = json;
        }
        else
        {
            newJson = "{ \"array\": " + json + "}";
        }
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

[Serializable]
public class ShopData
{
    public string code;
    public int type;
    public int cost;
    public string name;
    public string resource;
}