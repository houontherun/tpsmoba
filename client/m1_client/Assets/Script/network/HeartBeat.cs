/********************************************************************************
** auth： panyinglong
** date： 2017/5/16
** desc： 心跳&时间同步
*********************************************************************************/

using UnityEngine;
using System;
using System.IO;
using ProtoBuf;
public class HeartBeat
{
    private TimerInfo timer;
    public Connection connection { get; private set; }

    public bool isStart { get { return timer != null; } }
    public bool isReady { get; private set; }
    public long deltaTime { get; private set; }     // servertime - clienttime
    public long delayTime { get; private set; }     // 网络延迟时间
    private long minDelay = long.MaxValue;

    public int heartBeatSec = 6;
    public long LastHeartBeatTime { get; private set; }

    public int deathReportInterval = 10; // 每间隔N秒向逻辑端抛一次OnHeartDeath事件
    private long lastReportDeathTime = 0;

    public long ServerTimestamp
    {
        get
        {
            long now = GetTimestamp();
            return now + deltaTime;
        }
    }
    
    public long GetTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

    public long GetSecondTimestamp()
    {
        return (GetTimestamp() / 1000);
    }
    public uint ServerSecondTimestamp { get { return (uint)(ServerTimestamp / 1000); } }

    // 计算从现在到服务器的servertime需要多少毫秒
    public long GetTimespan(long servertime)
    {
        long span = servertime - ServerTimestamp;
        return span;
    }
    public int GetTimespanSeconds(uint serverSecondsTime)
    {
        return (int)(serverSecondsTime - ServerSecondTimestamp);
    }

    public HeartBeat(Connection con)
    {
        connection = con;
    }


    void Tick()
    {
        SendPingMessage();

        if (isReady)
        {
            long beat = GetTimestamp() - LastHeartBeatTime;
            long report = GetTimestamp() - lastReportDeathTime;
            if (beat > (heartBeatSec * 1000) && report > (deathReportInterval * 1000))
            {
                //Util.CallMethod("ConnectionManager", "OnHeartDeath", connection, beat);
                lastReportDeathTime = GetTimestamp();
                return;
            }
        }
    }
    //  指定时间间隔发送
    public void StartSyncTime()
    {
        Debug.Log(string.Format("开始对时 connection:{0} ", connection.name));
        StopSyncTime();
        if (timer == null)
        {
            SendPingMessage();
            timer = new TimerInfo(1, 0, Tick);
            AppFacade.Instance.timerManager.AddTimerEvent(timer);
        }
    }
    public void StopSyncTime()
    {
        if (timer != null)
        {
            AppFacade.Instance.timerManager.RemoveTimerEvent(timer);
        }
        timer = null;
        isReady = false;
        minDelay = long.MaxValue;
    }

    void updateDeltaTime(long clientTime, long serverTime)
    {
        long client_now = GetTimestamp();
        delayTime = (client_now - clientTime) / 2;
        if (minDelay > delayTime)
        {
            minDelay = delayTime;
            long server_now = serverTime + delayTime;
            deltaTime = server_now - client_now;
        }
        //Debug.Log(string.Format("---- update ping delta time:{0}", deltaTime));
    }

    public void SendPingMessage()       //向客户端时间戳发给服务端
    {
        if(connection.state != ConnectState.Success)
        {
            return;
        }
        //Debug.Log("SendPingMessage");

    }
    public void OnPingPack(byte[] data)
    {

    }

    public void SendPingBackMessage(long serverTime)       //向服务端返回服务端时间戳
    {
       
    }
}