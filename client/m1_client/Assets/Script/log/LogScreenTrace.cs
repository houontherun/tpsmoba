using UnityEngine;
using System.Collections.Generic;


//////////////////////////////////////////////////////////////////////////
/// <summary>
/// LOG 屏幕输出
/// </summary>
public class LogScreenTrace : LogTrace
{
    readonly char[] SPLIT = { '\n' };

    // 样式列表
    private GUIStyle[] m_StyleList;

    // 位置信息
    private Rect[] m_RectList;
    private byte m_nRectCount;

    // 信息列表
    Queue<LogTraceNode> m_ShowList;

    public LogScreenTrace()
    {
        m_StyleList = new GUIStyle[5];

        m_ShowList = new Queue<LogTraceNode>();
    }

    /// <summary>
    /// 获取类型ID
    /// </summary>
    public override LogTraceType Type { get { return LogTraceType.Screen; } }

    /// <summary>
    /// 初始化
    /// </summary>
    public override bool Init()
    {
        // 普通样式
        GUIStyle style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(0, 1, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Log] = style;

        // 警告样式
        style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(1, 1, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Warning] = style;

        // 错误样式
        style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(1, 0, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Error] = style;

        // 计算Label的位置
        InitRects();

        return true;
    }

    /// <summary>
    /// 添加日志
    /// </summary>
    public override void AddLog(LogTraceNode node)
    {
        // 有换行的，要把换行的劈成多段添加
        string[] split = node.msg.Split(SPLIT);
        for (int i = 0; i < split.Length; i++)
        {
            LogTraceNode newNode;
            newNode.msg = split[i];
            newNode.type = node.type;
            m_ShowList.Enqueue(newNode);
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public override void Update()
    {
        // 如果超过了最大值，要把最上面的弹出
        int nRemoveCount = 0;
        while ((byte)m_ShowList.Count > m_nRectCount)
        {
            m_ShowList.Dequeue();

            if (nRemoveCount++ > 5)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public override void OnGUI()
    {
        byte i = 0;
        foreach (LogTraceNode node in m_ShowList)
        {
            GUI.Label(m_RectList[i], node.msg, m_StyleList[node.type]);
            if (++i >= m_nRectCount)
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // 私有函数
    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 初始化位置信息
    /// </summary>
    private void InitRects()
    {
        // 只取1/3的屏幕显示内容
        float fHeight = Screen.height * (2.5f / 3f);
        float fYStart = (Screen.height - fHeight) / 2f;
        float fRectWidth = Screen.width - 40f;

        // 计算可以显示的条数
        m_nRectCount = (byte)(fHeight / 14f);

        m_RectList = new Rect[m_nRectCount];
        for (byte i = 0; i < m_nRectCount; i++)
        {
            m_RectList[i] = new Rect(20f, fYStart, fRectWidth, 14f);

            fYStart += 14f;
        }
    }




}