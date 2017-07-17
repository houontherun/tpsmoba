using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System.Reflection;
using System.Threading;

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
        pSceneTableInfo = AsyncLoadTable<S1TableConfig>();
        pSceneEleTableInfo = AsyncLoadTable<ElementTableConfig>();
        pDungeonTableInfo = AsyncLoadTable<DungeonTableConfig>();
    }

    public static void Init(Action kCallBack)
    {
        if (m_kLoadData.Count > 1) return;
        PreInit();
        Thread kThread = new Thread(new ParameterizedThreadStart(TreadInit));
        kThread.Start(kCallBack);
        
    }

    public static void TreadInit(object param)
    {
        Action kCallBack = (Action)param;
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

    static S1TableConfig pSceneTableInfo = null;
    public static S1TableConfig PSceneTableInfo
    {
        get
        {
            return pSceneTableInfo;
        }
    }

    static ElementTableConfig pSceneEleTableInfo = null;
    public static ElementTableConfig PSceneEleTableInfo
    {
        get
        {
            return pSceneEleTableInfo;
        }
    }

    static DungeonTableConfig pDungeonTableInfo = null;
    public static DungeonTableConfig PDungeonTableInfo
    {
        get
        {
            return pDungeonTableInfo;
        }
    }
}
