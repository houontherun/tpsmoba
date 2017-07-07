/********************************************************************************
** auth： panyinglong
** date： 2017/2/15
** desc： 服务器实例
*********************************************************************************/

using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

public enum ConnectState
{
    Unknown = 0,
    Success = 1,    // 连接成功
    Failed = 2,     // 连接失败
    Error = 3,      // read或write消息时出错 
    Close = 4,
}
public class Connection
{
    private Queue<KeyValuePair<int, byte[]>> sEvents = new Queue<KeyValuePair<int, byte[]>>();
    private static readonly object obj = new object();

    public string name { get; private set; }

    public string IP { get; private set; }

    public int PORT { get; private set; }

    public long deltaTime { get { return heartBeat.deltaTime; } }
    public long delayTime { get { return heartBeat.delayTime; } }

    public long ServerTimestamp { get { return heartBeat.ServerTimestamp; } }

    public long ServerSecondTimestamp { get { return (uint)(ServerTimestamp / 1000); } }

    public long GetSecondTimestamp()
    {
        return (GetTimestamp() / 1000);
    }
    public long GetTimestamp()
    {
        return heartBeat.GetTimestamp();
    }

    // 计算从现在到服务器的servertime需要多少毫秒
    public int GetTimespanSeconds(uint serverSecondsTime)
    {
        return (int)(serverSecondsTime - ServerSecondTimestamp);
    }

    public SocketClient socketClient { get; private set; }
    public HeartBeat heartBeat { get; private set; }
    public Connection(string n, bool needSyncTime = true)
    {
        name = n;
        socketClient = new SocketClient(this);
        heartBeat = new HeartBeat(this);
        NeedSyncTime = needSyncTime;
    }

    private ConnectState lastState;

    public ConnectState state { get; set; }

    private Queue<ConnectState> states = new Queue<ConnectState>();
    public Action<ConnectState, string> OnStateChanged = null;
    public bool NeedSyncTime = true; // 是否需要对时
    public void Update()
    {
        if (NeedSyncTime && state == ConnectState.Success)
        {
            if (!heartBeat.isStart)
            {
                heartBeat.StartSyncTime();
                return;
            }
            if (!heartBeat.isReady)
            {
                return;
            }
        }
        if (lastState != state)
        {
            states.Enqueue(state);
            lastState = state;
        }
        while(states.Count > 0)
        {
            ConnectState s = states.Dequeue();
            string msg = "";
            if(s == ConnectState.Error)
            {
                msg = socketClient.errorMsg;
            }
            else if(s == ConnectState.Failed)
            {
                msg = socketClient.failedMsg;
            }
            if(OnStateChanged != null)
            {
                OnStateChanged(s, msg);
            }
        }
        Dispatch();
    }
    
    public void Connect(string ip, int port, int timeout)
    {
        if (string.IsNullOrEmpty(ip) || port <= 0)
        {
            return;
        }
        sEvents.Clear();
        heartBeat.StopSyncTime();
        IP = ip;
        PORT = port;
        lastState = ConnectState.Unknown;
        state = ConnectState.Unknown;
        socketClient.ConnectServer(IP, PORT, timeout);
    }

    public bool SyncTime()
    {
        if(state == ConnectState.Success)
        {
            heartBeat.StartSyncTime();
            return true;
        }
        return false;
    }

    public Action<int, object> OnDefaultMessage;
    
    // 发送数据包到服务器，应用层调用
    public bool Send(int action, byte[] data, int serviceType = 0)
    {        
        socketClient.SendMessage(action, data, serviceType);
        return true;
    }
    
    // 关闭网络模块，关闭长连接
    public void Close()
    {
        lastState = ConnectState.Unknown;
        socketClient.Close();
        heartBeat.StopSyncTime();
    }

    internal void Enqueue(int action, byte[] stream)
    {
        lock (obj)
        {
            sEvents.Enqueue(new KeyValuePair<int, byte[]>(action, stream));
        }
    }

    internal void OnPingBack(byte[] data)
    {
        heartBeat.OnPingPack(data);
    }
    private void Dispatch()
    {
        while (sEvents.Count > 0)
        {
            if (LoadSceneManager.Instance().Isloading())
            {
                break;
            }
            lock (obj)
            {
                KeyValuePair<int, byte[]> msg = sEvents.Dequeue();
                DispatchNetMessage(msg.Key, msg.Value);
            }
        }
    }

    void DispatchNetMessage(int action, byte[] data)
    {
       
    }
}

