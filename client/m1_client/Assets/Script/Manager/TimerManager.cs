using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class TimerManager : Manager
{
    private List<TimerInfo> timers = new List<TimerInfo>();

    private bool isRunning = false;
    
    public float Interval { get; set; }

    // Use this for initialization
    void Start()
    {
        //StartTimer(AppConst.TimerInterval);
    }

    /// <summary>
    /// 启动计时器
    /// </summary>
    /// <param name="interval"></param>
    public void StartTimer(float value)
    {
        if (isRunning)
        {
            StopTimer();
        }
        Interval = value;
        InvokeRepeating("Run", 0, Interval);
        isRunning = true;
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void StopTimer()
    {
        CancelInvoke("Run");
        isRunning = false;
    }

    /// <summary>
    /// 添加计时器事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="o"></param>
    public void AddTimerEvent(TimerInfo info)
    {
        if (!timers.Contains(info))
        {
            timers.Add(info);
        }
    }

    /// <summary>
    /// 删除计时器事件
    /// </summary>
    /// <param name="name"></param>
    public void RemoveTimerEvent(TimerInfo info)
    {
        if (timers.Contains(info) && info != null)
        {
            info.timerState = TimerState.Delete;
        }
    }

    /// <summary>
    /// 停止计时器事件
    /// </summary>
    /// <param name="info"></param>
    public void StopTimerEvent(TimerInfo info)
    {
        if (timers.Contains(info) && info != null)
        {
            info.timerState = TimerState.Stop;
        }
    }

    /// <summary>
    /// 继续计时器事件
    /// </summary>
    /// <param name="info"></param>
    public void ResumeTimerEvent(TimerInfo info)
    {
        if (timers.Contains(info) && info != null)
        {
            info.timerState = TimerState.Run;
        }
    }

    /// <summary>
    /// 计时器运行
    /// </summary>
    void Run()
    {
        if (timers.Count == 0)
        {
            return;
        }
            
        for (int i = 0; i < timers.Count; i++)
        {
            TimerInfo o = timers[i];
            if (o.timerState == TimerState.Delete || o.timerState == TimerState.Stop)
            {
                continue;
            }
            o.Tick(Interval);
        }
        /////////////////////////清除标记为删除的事件///////////////////////////
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            if (timers[i].timerState == TimerState.Delete)
            {
                timers.Remove(timers[i]);
            }
        }
    }
}

public enum TimerState
{
    Run,
    Stop,
    Delete
}

public class TimerInfo
{
    private enum TimerType
    {
        Forever,    // 无限次循环
        Numeral     // 有限次循环
    }
    private float tick;
    private object target;
    private float interval;
    private int loop = 0;
    private Action action;
    private TimerType timerType;
    
    public TimerState timerState;

    public TimerInfo(float interval, int loop, Action action)
    {
        this.interval = interval;
        this.action = action;
        this.loop = loop;
        timerState = TimerState.Run;
    }
    public void SetInterval(float interval)
    {
        if (interval > 0)
        {
            this.interval = interval;
        }
    }
    public void Tick(float deltaTime)
    {
        if (tick >= interval)
        {
            if (action != null)
            {
                action();
            }
            tick = 0;

            if (timerType == TimerType.Numeral)
            {
                loop--;
                if (loop <= 0)
                {
                    timerState = TimerState.Delete;
                   
                }
            }
        }
        tick += deltaTime;
    }
    public void Reset()
    {
        tick = 0;
    }
}