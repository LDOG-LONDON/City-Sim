using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour {

    private static T _instance;
    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " wont create again - returning null.");
                return null;
            }

            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Somthing went really wrong " +
                               " - there should never be more than 1 singleton!" +
                               "Reopenning the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                    return _instance;
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// This is meant to make sure Unity (or some other object) doesnt destroy
    /// the singleton before its ready
    /// </summary>
	public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
