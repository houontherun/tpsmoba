
using System;
using UnityEngine;
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;
    private static GameObject _InstanceRoot = null;
    public static void CreateInstance()
    {
        if (SingletonBehaviour<T>._Instance == null)
        {
            if (SingletonBehaviour<T>._Instance == null)
            {
                if (_InstanceRoot == null)
                {
                    _InstanceRoot = new GameObject();
                    _InstanceRoot.name = "_singleton_root";
                    GameObject.DontDestroyOnLoad(_InstanceRoot);
                }

                SingletonBehaviour<T>._Instance = _InstanceRoot.AddComponent<T>();
            }
        }
    }

    public static void DestroyInstance()
    {
        if (SingletonBehaviour<T>._Instance != null)
        {
            GameObject.DestroyObject(SingletonBehaviour<T>._Instance);
            SingletonBehaviour<T>._Instance = null;
        }
    }

    public static T GetInstance()
    {
        CreateInstance();
        return SingletonBehaviour<T>._Instance;
    }

    public static T GetInstance(int i)
    {
        if ((i >= 0) && (i < 1))
        {
            return SingletonBehaviour<T>._Instance;
        }
        return null;
    }

    public static T Instance
    {
        get
        {
            CreateInstance();
            return SingletonBehaviour<T>._Instance;
        }
    }
}


