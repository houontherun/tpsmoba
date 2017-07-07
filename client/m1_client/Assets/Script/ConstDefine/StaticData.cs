using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System.Reflection;

public class TableData 
{
    public class LoadData
    {
        public object kObj = null;
        public Type kType = null;
        public string text = "";
    }
    static List<LoadData> m_kLoadData = new List<LoadData>();

    public static T AsyncLoadTable<T>() where T : new()
    {
        LoadData kLoadData = new LoadData();
        kLoadData.kObj = new T();
        kLoadData.kType = typeof(T);
        MethodInfo kMethodInfo = kLoadData.kType.GetMethod("GetTableName", BindingFlags.Instance | BindingFlags.Public);
        string kTableName = (string)kMethodInfo.Invoke(kLoadData.kObj, null);
        kLoadData.text = Resources.Load<TextAsset>(kTableName).text;
        if (kLoadData.text != null)
        {
            m_kLoadData.Add(kLoadData);
        }
        return (T)kLoadData.kObj;
    }


    public static void PreInit()
    {
        pSceneTableInfo = AsyncLoadTable<SceneTable>();
    }

    public static void Init(Action kCallBack)
    {
        PreInit();
    }

    static SceneTable pSceneTableInfo = null;
    public static SceneTable PSceneTableInfo
    {
        get
        {
            return pSceneTableInfo;
        }
    }

    static SceneElementTable pSceneEleTableInfo = null;
    public static SceneElementTable PSceneEleTableInfo
    {
        get
        {
            return pSceneEleTableInfo;
        }
    }
}
