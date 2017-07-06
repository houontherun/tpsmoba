using UnityEngine;
using System;

public class Singleton<T> where T:class, new ()
{
    private static T instance_ = null;

    public static T Instance()
    {
        if(instance_ == null)
        {
            instance_ = Activator.CreateInstance<T>();
            if(instance_ == null)
            {
                Debug.LogError("create instance " + typeof(T) + " fail");
            }
        }
        return instance_;
    }
}
