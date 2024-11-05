using UnityEngine;
using DG.Tweening;
using Fusion;

public enum SceneName
{
    Developer,
    Title,
    Notice,
    Lobby,
    Game
}

public class App : Singleton<App>
{
    [SerializeField] GameObject runnerObj;
    public static NetworkRunner Runner { get; private set; }

    private ViewManager view;
    private SimManager sim;
    private UIManager ui;

    private SoundManager sound;
    private NetworkManager network;
    private PlayerManager player;
    private GameManager game;

    private TitleData titleData;
    private PlayerData playerData;
    private SettingData settingData;
    private ClanData clanData;

    #region Getter Setter
    public partial class Manager
    {
        public static UIManager UI => instance.ui;
        public static ViewManager View => instance.view;
        public static SimManager Sim => instance.sim;

        public static SoundManager Sound => instance.sound;
        public static NetworkManager Network => instance.network;
        public static PlayerManager Player => instance.player;
        public static GameManager Game => instance.game;
    }

    public partial class Data
    {
        public static SettingData Setting => instance.settingData;
        public static TitleData Title => instance.titleData;

        public static PlayerData Player => instance.playerData;
        public static ClanData Clan => instance.clanData;
    }
    #endregion

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;

        DOTween.safeModeLogBehaviour = DG.Tweening.Core.Enums.SafeModeLogBehaviour.Error;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Runner == null)
        {
            Runner = Instantiate(runnerObj, transform).GetComponent<NetworkRunner>();
        }
    }

    public static void LoadScene(SceneName sceneName)
    {
        DOTween.KillAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)sceneName);
    }

    #region Get View As T
    public static T ViewManagerAs<T>(object manager) where T : MonoBehaviour
    {
        if (manager == null || manager is not T)
        {
            return null;
        }

        return manager as T;
    }

    public static T GetViewAs<T>() where T : MonoBehaviour
    {
        return ViewManagerAs<T>(Manager.View);
    }

    public static T GetSimAs<T>() where T : MonoBehaviour
    {
        return ViewManagerAs<T>(Manager.Sim);
    }

    public static T GetUIAs<T>() where T : MonoBehaviour
    {
        return ViewManagerAs<T>(Manager.UI);
    }
    #endregion

    #region  Get ViewManager
    public static DeveloperManager GetDeveloperManager()
    {
        return GetViewAs<DeveloperManager>();
    }

    public static TitleManager GetTitleManager()
    {
        return GetViewAs<TitleManager>();
    }

    public static LobbyManager GetLobbyManager()
    {
        return GetViewAs<LobbyManager>();
    }

    public partial class Manager
    {
        public static DeveloperManager Developer { get => GetDeveloperManager(); }
        public static TitleManager Title { get => GetTitleManager(); }
        public static LobbyManager Lobby { get => GetLobbyManager(); }
    }
    #endregion

    #region  Get SimManager
    public static ReadyManager GetReadyManager()
    {
        return GetViewAs<ReadyManager>();
    }

    public partial class Manager
    {
        public static ReadyManager Ready { get => GetReadyManager(); }
    }
    #endregion

    #region  Get UIManager
    public static TitleUIManager GetTitleUIManager()
    {
        return GetUIAs<TitleUIManager>();
    }

    public static LobbyUIManager GetLobbyUIManager()
    {
        return GetUIAs<LobbyUIManager>();
    }

    public static ReadyUIManager GetReadyUIManager()
    {
        return GetUIAs<ReadyUIManager>();
    }

    public static GameUIManager GetGameUIManager()
    {
        return GetUIAs<GameUIManager>();
    }

    public class UI
    {
        public static TitleUIManager Title { get => GetTitleUIManager(); }
        public static LobbyUIManager Lobby { get => GetLobbyUIManager(); }
        public static ReadyUIManager Ready { get => GetReadyUIManager(); }
        public static GameUIManager Game { get => GetGameUIManager(); }
    }
    #endregion
}
