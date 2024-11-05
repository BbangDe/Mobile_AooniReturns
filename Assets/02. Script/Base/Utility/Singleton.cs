using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T _instance = null;
    static bool IsDestroy = false;

    public static T instance
    {
        get
        {
            if (IsDestroy)
                return null;
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    if (_instance == null)
                    {
                        // Error when an instance cannot be created.
                        Debug.LogError("Singleton::Problem during the creation of " + typeof(T).ToString());
                    }
                }
                _instance.Init();
            }
            return _instance;
        }
    }

    public static bool IsCreateInstance()
    {
        if (_instance == null)
            return false;
        return true;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            _instance.Init();
        }
    }

    /// <summary>
    /// Create Singleton
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Clear Singleton
    /// </summary>
    public virtual void Destory() { }

    private void Release()
    {
        if (_instance != null)
        {
            Debug.LogFormat("Singleton Release : {0}", name);
            _instance.Destory();
            Destroy(_instance.gameObject);
            _instance = null;
        }
        else
            Debug.LogFormat("Singleton Release null : {0}", name);

    }

    /// <summary>
    /// Clear GameObject
    /// </summary>
    private void OnDestroy()
    {
        Release();
    }
}
