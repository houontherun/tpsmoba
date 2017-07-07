
public enum LogTraceType
{
    Base = -1,  // ��������

    Screen,     // �������Ļ
    File,       // ������ļ�
    Control,    // ����̨
    Max,
}


/// <summary>
/// LOG�������
/// </summary>
public class LogTrace
{
    public LogTrace()
    {

    }

    /// <summary>
    /// ��ȡ����ID
    /// </summary>
    public virtual LogTraceType Type { get { return LogTraceType.Base; } }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public virtual bool Init() { return true; }

    /// <summary>
    /// �����־
    /// </summary>
    public virtual void AddLog(LogTraceNode node) { }

    /// <summary>
    /// ִ��
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// GUI
    /// </summary>
    public virtual void OnGUI() { }

    /// <summary>
    /// �ͷ�
    /// </summary>
    public virtual void Release() { }
}