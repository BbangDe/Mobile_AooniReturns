using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public struct SoundData
{
    public float Master;
    public float BGM;
    public float SFX;

    public bool MasterMuted;
    public bool BGMMuted;
    public bool SFXMuted;
}

public class SettingData : Manager
{
    public struct GameData
    {
        public string UDID;
        public string Version;
        public SystemLanguage Language;

        public SoundData Sound;
    }

    [SerializeField] SettingDefault Default;

    public SoundData DefaultSound => Default.Sound;

    private GameData Data;
    private string dataPath;

    public bool IsDataLoaded { get; private set; }
    public bool IsDataSaved { get; private set; }

    private bool IsDataMessy;

    #region Getter Setter
    public SystemLanguage Language
    {
        get => Data.Language;
        set
        {
            Data.Language = value;
            IsDataSaved = false;
            IsDataMessy = true;
            SaveToLocal();
        }
    }

    public SoundData Sound
    {
        get => Data.Sound;
        set
        {
            Data.Sound = value;
            IsDataSaved = false;
            IsDataMessy = true;
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        IsDataLoaded = false;
        IsDataSaved = false;
        IsDataMessy = true;
        dataPath = Application.persistentDataPath + '/' + Default.Path;

        LoadFromLocal();
    }

    private void OnApplicationQuit()
    {
        SaveToLocal();
    }

    private void CreateDefault()
    {
        Data = new()
        {
            UDID = SystemInfo.deviceUniqueIdentifier,
            Version = Application.version,
            Language = SystemLanguage.English,

            Sound = DefaultSound,
        };

        Data.Language = Application.systemLanguage switch
        {
            SystemLanguage.Korean => SystemLanguage.Korean,
            SystemLanguage.Chinese => SystemLanguage.Chinese,
            SystemLanguage.Japanese => SystemLanguage.Japanese,
            _ => SystemLanguage.English
        };

        SaveToLocal();

        IsDataLoaded = true;
        IsDataMessy = false;
    }

    private void LoadFromLocal()
    {
        try
        {
            using var fstream = new FileStream(dataPath, FileMode.Open);
            LoadDataInternal(fstream);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to load save file from " + dataPath +
                ". Default settings will be applied.\n" + exception);
        }

        if (IsDataLoaded)
        {
            Debug.Log("Data loaded successfully from " + dataPath);
        }
        else
        {
            CreateDefault();
        }

        IsDataMessy = false;
    }

    private void LoadDataInternal(FileStream fstream) // loading save file should not be async function
    {
        using var streamReader = new StreamReader(fstream);
        string jsonStr = streamReader.ReadToEnd();

        Data = JsonUtility.FromJson<GameData>(jsonStr);

        if (!Data.UDID.Equals(SystemInfo.deviceUniqueIdentifier))
        {
            // TODO: just reset screen related data..?
            throw new InvalidDataException("New device detected");
        }

        if (!Data.Version.Equals(Application.version))
        {
            // TODO: notice user of version mismatch
            Data.Version = Application.version;
        }

        IsDataLoaded = true;
    }

    public async void SaveToLocal(bool _force = false)
    {
        // If the data hasn't changed, we don't need to save it again
        if (!IsDataMessy && !_force) return;

        using (var fstream = new FileStream(dataPath, FileMode.Create))
        {
            do
            {
                IsDataMessy = false;

                try
                {
                    await SaveDataInternal(fstream);
                }
                catch (Exception exception)
                {
                    Debug.LogError("ERROR: Failed to save file.\n" + exception);
                    return;
                }
            }
            while (IsDataMessy);
        }

        IsDataSaved = true;
        Debug.Log("Data saved successfully to " + dataPath);
    }

    public async Task SaveDataInternal(FileStream fstream)
    {
        string jsonStr = JsonUtility.ToJson(Data);

        using var streamWriter = new StreamWriter(fstream);
        await streamWriter.WriteAsync(jsonStr);
    }
}
