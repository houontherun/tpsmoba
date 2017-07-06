using UnityEngine;
using System.Collections.Generic;


//////////////////////////////////////////////////////////////////////////
/// <summary>
/// LOG ��Ļ���
/// </summary>
public class LogScreenTrace : LogTrace
{
    readonly char[] SPLIT = { '\n' };

    // ��ʽ�б�
    private GUIStyle[] m_StyleList;

    // λ����Ϣ
    private Rect[] m_RectList;
    private byte m_nRectCount;

    // ��Ϣ�б�
    Queue<LogTraceNode> m_ShowList;

    public LogScreenTrace()
    {
        m_StyleList = new GUIStyle[5];

        m_ShowList = new Queue<LogTraceNode>();
    }

    /// <summary>
    /// ��ȡ����ID
    /// </summary>
    public override LogTraceType Type { get { return LogTraceType.Screen; } }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public override bool Init()
    {
        // ��ͨ��ʽ
        GUIStyle style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(0, 1, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Log] = style;

        // ������ʽ
        style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(1, 1, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Warning] = style;

        // ������ʽ
        style = new GUIStyle();
        style.normal.background = null;
        style.normal.textColor = new Color(1, 0, 0);
        style.fontSize = 14;
        m_StyleList[(int)LogType.Error] = style;

        // ����Label��λ��
        InitRects();

        return true;
    }

    /// <summary>
    /// �����־
    /// </summary>
    public override void AddLog(LogTraceNode node)
    {
        // �л��еģ�Ҫ�ѻ��е����ɶ�����
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
    /// ִ��
    /// </summary>
    public override void Update()
    {
        // ������������ֵ��Ҫ��������ĵ���
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
    /// ִ��
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
    // ˽�к���
    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ��ʼ��λ����Ϣ
    /// </summary>
    private void InitRects()
    {
        // ֻȡ1/3����Ļ��ʾ����
        float fHeight = Screen.height * (2.5f / 3f);
        float fYStart = (Screen.height - fHeight) / 2f;
        float fRectWidth = Screen.width - 40f;

        // ���������ʾ������
        m_nRectCount = (byte)(fHeight / 14f);

        m_RectList = new Rect[m_nRectCount];
        for (byte i = 0; i < m_nRectCount; i++)
        {
            m_RectList[i] = new Rect(20f, fYStart, fRectWidth, 14f);

            fYStart += 14f;
        }
    }




}