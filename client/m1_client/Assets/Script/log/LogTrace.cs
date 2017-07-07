
public enum LogTraceType
{
    Base = -1,  // 基类类型

    Screen,     // 输出到屏幕
    File,       // 输出到文件
    Control,    // 控制台
    Max,
}


/// <summary>
/// LOG输出基类
/// </summary>
public class LogTrace
{
    public LogTrace()
    {

    }

    /// <summary>
    /// 获取类型ID
    /// </summary>
    public virtual LogTraceType Type { get { return LogTraceType.Base; } }

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual bool Init() { return true; }

    /// <summary>
    /// 添加日志
    /// </summary>
    public virtual void AddLog(LogTraceNode node) { }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// GUI
    /// </summary>
    public virtual void OnGUI() { }

    /// <summary>
    /// 释放
    /// </summary>
    public virtual void Release() { }
}