using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


//////////////////////////////////////////////////////////////////////////
/// <summary>
/// LOG �����HTML�ļ�������Ϊ�����ܿ��ǣ�ʹ��һ���������߳����������е���־����Ϊ
/// д�ļ���������һ�����ıȽϴ�Ĳ���
/// </summary>
public class LogFileTrace : LogTrace
{
    const long len = 1024 * 1000; //byte

    long _curFileLength = -1;
    long curFileLength
    {
        get
        {
            if (_curFileLength == -1)
            {
                string file = getFileName();
                if (File.Exists(file))
                {
                    FileInfo fi = new FileInfo(file);
                    _curFileLength = fi.Length;
                }
                else
                {
                    _curFileLength = 0;
                }

            }
            return _curFileLength;
        }
        set
        {
            _curFileLength = value;
        }
    }
    private string newFileName()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        int index = 1;
        string fullName = Path.Combine(m_szFilePath, today + ".txt");
        while (true)
        {
            if (File.Exists(fullName))
            {
                fullName = Path.Combine(m_szFilePath, today + "_" + index.ToString() + ".txt");
                index++;
            }
            else
            {
                break;
            }
        }
        return fullName;
    }
    private string getFileName()
    {
        if (!Directory.Exists(m_szFilePath))
        {
            Directory.CreateDirectory(m_szFilePath);
        }
        DirectoryInfo di = new DirectoryInfo(m_szFilePath);
        FileInfo[] files = di.GetFiles("*.txt");
        if (files.Length > 0)
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            FileInfo latestFile = null;
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.Contains(today))
                {
                    continue;
                }
                if (latestFile == null)
                {
                    latestFile = files[i];
                    continue;
                }
                int ret = string.Compare(latestFile.Name, files[i].Name);
                if (ret < 0)
                {
                    latestFile = files[i];
                }
            }
            if (latestFile != null && latestFile.Length < len)
            {
                return latestFile.FullName;
            }
        }
        return newFileName();
    }
    /*****************************/
    // �ֳ��Ƿ��˳�
    public bool m_bThreadContinue;

    // ��Ԫ��
    object m_guard;

    // ��Ϣlist
    Queue<LogTraceNode> m_LogList;

    StreamWriter m_Writer;

    Thread m_LogThread;

    string m_szFilePath;

    public LogFileTrace()
    {
        m_bThreadContinue = true;
        m_guard = new object();
        m_LogList = new Queue<LogTraceNode>();
    }

    /// <summary>
    /// ��ȡ����ID
    /// </summary>
    public override LogTraceType Type { get { return LogTraceType.File; } }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public override bool Init()
    {
        // ��ͬģʽ��ѡ��ͬ���ļ�·��
        if(AppConst.PublishMode)
        {
            if (Application.isMobilePlatform)
                m_szFilePath = Application.persistentDataPath;
            else
                m_szFilePath = Application.dataPath;
        }
        else
        {
            DirectoryInfo path = new DirectoryInfo(Application.dataPath);
            m_szFilePath = Path.Combine(path.Parent.FullName, "log");
        }
        

        // ����д�ļ��߳�
        m_LogThread = new Thread(new ThreadStart(Run));
        m_LogThread.Start();

        return true;
    }

    /// <summary>
    /// �ͷ�
    /// </summary>
    public override void Release()
    {
        m_bThreadContinue = false;
    }

    /// <summary>
    /// �����־
    /// </summary>
    public override void AddLog(LogTraceNode node)
    {
        lock (m_guard)
        {
            m_LogList.Enqueue(node);
        }
    }

    void writeHtml()
    {
        FileStream file = new FileStream(string.Format("{0}//Log.html", m_szFilePath), FileMode.Create);
        m_Writer = new StreamWriter(file);
        //д��ͷ��Ϣ   
        m_Writer.Write("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>Log</title></head><body>");
        m_Writer.Flush();

        do
        {
            Thread.Sleep(1);

            // ���list���� Ҫ��ӡ�Ķ��� �� ���� ��ӡ
            lock (m_guard)
            {
                int nMsgCount = m_LogList.Count;
                if (nMsgCount > 0)
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        // ÿ�δ���30����������̫�ã���������߳������������̵߳ȴ�ʱ��̫��
                        if (j >= 30)
                            break;

                        LogTraceNode node = m_LogList.Dequeue();

                        switch ((LogType)node.type)
                        {
                            case LogType.Log:
                                m_Writer.Write(string.Format("<div style=\"color:#0000FF\">{0}</div>", node.msg));
                                break;

                            case LogType.Warning:
                                m_Writer.Write(string.Format("<div style=\"color:#FF00FF\">{0}</div>", node.msg));
                                break;

                            case LogType.Error:
                                m_Writer.Write(string.Format("<div style=\"color:#FF0000\">{0}</div>", node.msg));
                                break;
                        }
                    }

                    m_Writer.Flush();
                }
            }


        } while (m_bThreadContinue);

        // �˳��߳�֮ǰ����β����
        m_Writer.Write("</body></html>");

        //�ر���   
        m_Writer.Close();

        //������   
        m_Writer.Dispose();
    }
    void writeTxt(string name)
    {
        FileMode mode = FileMode.Create;
        if (File.Exists(name))
        {
            mode = FileMode.Append;
        }
        FileStream file = new FileStream(name, mode);
        m_Writer = new StreamWriter(file);
        m_Writer.Flush();
        do
        {
            Thread.Sleep(1);

            // ���list���� Ҫ��ӡ�Ķ��� �� ���� ��ӡ
            lock (m_guard)
            {
                int nMsgCount = m_LogList.Count;
                if (nMsgCount > 0)
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        // ÿ�δ���30����������̫�ã���������߳������������̵߳ȴ�ʱ��̫��
                        if (j >= 30)
                            break;

                        LogTraceNode node = m_LogList.Dequeue();

                        switch ((LogType)node.type)
                        {
                            case LogType.Log:
                                m_Writer.Write(node.msg + "\r\n");
                                break;

                            case LogType.Warning:
                                m_Writer.Write(node.msg + "\r\n");
                                break;

                            case LogType.Error:
                                m_Writer.Write(node.msg + "\r\n");
                                break;
                        }
                    }

                    m_Writer.Flush();
                }
            }


        } while (m_bThreadContinue);
        
        //�ر���   
        m_Writer.Close();

        //������   
        m_Writer.Dispose();
    }
    /// <summary>
    /// �̺߳���
    /// </summary>
    private void Run()
    {
        if (AppConst.PublishMode)
        {
            writeHtml();
        }
        else
        {
            //string name = getFileName();
            //writeTxt(name);
        }        
    }

}