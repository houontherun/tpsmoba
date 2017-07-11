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
        kLoadData.text = ResourceManager.GetText(kTableName);
        if (kLoadData.text != null)
        {
            m_kLoadData.Add(kLoadData);
        }
        return (T)kLoadData.kObj;
    }


    public static void PreInit()
    {
        pSceneTableInfo = AsyncLoadTable<SceneTableConfig>();
        pSceneEleTableInfo = AsyncLoadTable<SceneElementTableConfig>();
    }

    public static void Init(Action kCallBack)
    {
        PreInit();
        LoadData kLoadData = null;
        for (int iIdx = 0; iIdx < m_kLoadData.Count; iIdx++)
        {
            kLoadData = m_kLoadData[iIdx];
            MethodInfo kMethodInfo = kLoadData.kType.GetMethod("Load", BindingFlags.Instance | BindingFlags.Public);
            try
            {
                kMethodInfo.Invoke(kLoadData.kObj, new object[] { kLoadData.text });
            }
            catch (System.Exception ex)
            {
                LogCenter.LogError("json Load Err: [" + kLoadData.kType.ToString() + "]----->" + ex.Message);
                
            }
        }
        m_kLoadData.Clear();
        if (kCallBack != null)
        {
            kCallBack();
        }
    }


    static SceneTableConfig pSceneTableInfo = null;
    public static SceneTableConfig PSceneTableInfo
    {
        get
        {
            return pSceneTableInfo;
        }
    }

    static SceneElementTableConfig pSceneEleTableInfo = null;
    public static SceneElementTableConfig PSceneEleTableInfo
    {
        get
        {
            return pSceneEleTableInfo;
        }
    }
}
