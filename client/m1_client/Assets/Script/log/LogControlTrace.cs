/*******************************************************************
** 版  权:	深圳自游网络有限公司
** 创建人:	张呈鹏
** 日  期:	2014/12/20 15:56
** 版  本:	1.0
** 描  述:	LOG输出
** 应  用:  	主要用于打印游戏中输出的LOG到屏幕或者是到文件	
********************************************************************/
using UnityEngine;
/// <summary>
/// LOG输出控制台，编辑器专用
/// </summary>
public class LogControlTrace : LogTrace
{
    public LogControlTrace()
    {

    }

    /// <summary>
    /// 获取类型ID
    /// </summary>
    public override LogTraceType Type { get { return LogTraceType.Control; } }

    /// <summary>
    /// 添加日志
    /// </summary>
    public override void AddLog(LogTraceNode node)
    {
        switch ((LogType)node.type)
        {
            case LogType.Log:
                Debug.Log(node.msg);
                break;
            case LogType.Warning:
                Debug.LogWarning(node.msg);
                break;
            case LogType.Error:
                Debug.LogError(node.msg);
                break;
        }
    }
}