using UnityEngine;
using System.Collections.Generic;

using System.IO;
using System.Collections;
using System;
using System.Text;
using System.Linq;

public struct LogTraceNode
{
    public byte type;  // 输出类型
    public string msg;         // 数据
}

public class LogCenter : Singleton<LogCenter>
{
    // 日志列表
    Queue<LogTraceNode> m_LogList;

    // 当前激活的日志输出方式
    LogTrace m_CurTrace;

    /// <summary>
    /// 构造函数
    /// </summary>
    public LogCenter()
    {
        m_CurTrace = null;
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Release()
    {
        if (m_CurTrace != null)
        {
            m_CurTrace.Release();
            m_CurTrace = null;
        }
    }


    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        if (null != m_CurTrace)
            m_CurTrace.Update();
    }

    /// <summary>
    /// 由Main.cs调用渲染
    /// </summary>
    public void OnGUI()
    {
        if (null != m_CurTrace)
            m_CurTrace.OnGUI();
    }

    /// <summary>
    /// 压入一个日志 
    /// </summary>
    public void Push(LogType type, string msg)
    {
        if (m_CurTrace != null)
        {
            LogTraceNode node;
            node.type = (byte)type;
            node.msg = msg;
            if (AppConst.logDebug)
            {
                m_CurTrace.AddLog(node); 
            }
            else if(type != LogType.Log)
            {
                m_CurTrace.AddLog(node); 
            }
           
        }
    }

    /// <summary>
    /// 打开/关闭一个TRACE 
    /// </summary>
    public bool OpenTrace(LogTraceType type)
    {
        LogTrace newTrace = null;

        switch (type)
        {
            case LogTraceType.Screen:
                newTrace = new LogScreenTrace();
                break;

            case LogTraceType.File:
                newTrace = new LogFileTrace();
                break;

            case LogTraceType.Control:
                newTrace = new LogControlTrace();
                break;
            default:
                return false;
        }

        if (!newTrace.Init())
        {
            LogCenter.LogError("LogCenter::OpenTrace init error, type=" + type);
            return false;
        }

        // 如果当前有，要把之前的释放掉
        if (m_CurTrace != null)
        {
            m_CurTrace.Release();
        }

        m_CurTrace = newTrace;
        return true;
    }

    /// <summary>
    /// 关闭当前TRACE
    /// </summary>
    public void Close()
    {
        if (m_CurTrace != null)
        {
            m_CurTrace.Release();
            m_CurTrace = null;
        }
    }
    public static void Log(string msg)
    {
        LogCenter.Instance().Push(LogType.Log, msg);
    }

    /// <summary>
    /// 警告LOG，使用这个打印LOG时避免高频率调用，有可能会影响帧数下降 
    /// </summary>
    public static void LogWarning(string msg)
    {
        LogCenter.Instance().Push(LogType.Warning, msg);
    }

    /// <summary>
    /// 错误LOG，使用这个打印LOG时避免高频率调用，有可能会影响帧数下降
    /// </summary>
    public static void LogError(string msg)
    {
        LogCenter.Instance().Push(LogType.Error, msg);
    }
}
