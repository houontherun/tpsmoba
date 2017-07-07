using UnityEngine;
using System.Collections;

public class NotiConst
{
    /// <summary>
    /// Controller层消息通知
    /// </summary>
    public const string START_UP = "StartUp";                       //启动框架
    public const string DISPATCH_MESSAGE = "DispatchMessage";       //派发信息

    /// <summary>
    /// View层消息通知
    /// </summary>
    public const string UPDATE_MESSAGE = "UpdateMessage";           //更新消息
    public const string UPDATE_EXTRACT = "UpdateExtract";           //更新解包
    public const string UPDATE_DOWNLOAD = "UpdateDownload";         //更新下载
    public const string UPDATE_PROGRESS = "UpdateProgress";         //更新进度
    public const string UPDATE_FINISHED = "UpdateFinished";         //更新完成

    public const string LOADING_START = "LoadingStart";             //初始化loading条
    public const string LOGIN_START = "LoginPanelStart";             //初始化loading条
    public const string LOADING_End = "LoadingEnd";             //初始化loading条
}
