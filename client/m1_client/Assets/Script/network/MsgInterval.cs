using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MsgInterval
{
    public MsgInterval()
    {
        InitInterval();
    }

    protected Dictionary<int, float> interval_ = new Dictionary<int, float>();
    Dictionary<int, float> last_time_ = new Dictionary<int, float>();
   // TipsBase waiting_tips_ = null;
    Dictionary<int, int> waiting_key_ = new Dictionary<int, int>();
    TimerInfo timer_ = null;

    protected virtual void InitInterval()
    {

    }

    protected bool BeyondInterval(int key,int curWatingKey)
    {
        float iTime = 0f;
        if(!interval_.TryGetValue(key,out iTime))
        {
            return true;
        }
        float lastTime = 0f;
        float nowTime = Time.realtimeSinceStartup;
        if (!last_time_.TryGetValue(key, out lastTime))
        {
            last_time_.Add(key, nowTime);
            StartTimer(curWatingKey, iTime);
            return true;
        }
        else
        {
            if (lastTime + iTime <= nowTime)
            {
                last_time_[key] = nowTime;
                StartTimer(curWatingKey, iTime);
                return true;
            }
        }
        return false;
    }

    void StartTimer(int waitingKey, float iTime)
    {
        iTime = 6f;
        if(!waiting_key_.ContainsKey(waitingKey))
        {
            waiting_key_.Add(waitingKey, 1);
        }
        //if (null == waiting_tips_)
        //{
        //    TipsManager tm = Singleton<UIStubLogic>.Instance().GetTipsManager();
        //    waiting_tips_ = new WaitingNetworkTips(tm);
        //    tm.ShowNornalTips(waiting_tips_);
        //}
        if(null == timer_)
        {
            timer_ = new TimerInfo(iTime, 1, delegate () {
                OnTimer();
            });
            AppFacade.Instance.timerManager.AddTimerEvent(timer_);
        }
    }

    public virtual void OnRevieveMsg(int key)
    {
        if (waiting_key_.ContainsKey(key))
        {
            waiting_key_.Remove(key);
            if (0 == waiting_key_.Count)
            {
                StopTimer();
            }
        }
    }

    void OnTimer()
    {
        StopTimer();
    }

    void StopTimer()
    {
        //if(null != waiting_tips_)
        //{
        //    TipsBase.Close(waiting_tips_);
        //    waiting_tips_ = null;
        //}
        if(null != timer_)
        {
            AppFacade.Instance.timerManager.RemoveTimerEvent(timer_);
        }
        waiting_key_.Clear();
    }
}

public class ModuleMsgInterval : MsgInterval
{
    protected override void InitInterval()
    {
        
    }

    public bool BeyondInterval(int action)
    {
        int key = MakeKey(action);
        return base.BeyondInterval(key, key);
    }

    public override void OnRevieveMsg(int action)
    {
        int key = MakeKey(action);
        base.OnRevieveMsg(key);
    }

    int MakeKey(int action)
    {
        return action;
    }
}

public class LuaMsgInterval : MsgInterval
{
    Dictionary<string, int> string_to_key_ = new Dictionary<string, int>();
    int id_maker_ = 0;
    Dictionary<int, int> key_to_old_serial_ = new Dictionary<int, int>();
    Dictionary<int, int> old_serial_to_key_ = new Dictionary<int, int>();
    Dictionary<int, int> new_to_old_serial_ = new Dictionary<int, int>();

    protected override void InitInterval()
    {
        id_maker_ = 0;
       
    }

    public bool BeyondInterval(string reqCmd, int luaSerial)
    {
        int key = MakeKey(reqCmd);
        int oldSerial = 0;
        if (!key_to_old_serial_.TryGetValue(key,out oldSerial))
        {
            new_to_old_serial_.Add(luaSerial,luaSerial);
            old_serial_to_key_.Add(luaSerial,key);
            key_to_old_serial_.Add(key, luaSerial);
            oldSerial = luaSerial;
        }
        else
        {
            new_to_old_serial_.Add(luaSerial, oldSerial);
        }
        return base.BeyondInterval(key, oldSerial);
    }

    int MakeKey(string reqCmd)
    {
        int k = 0;
        if(string_to_key_.TryGetValue(reqCmd,out k))
        {
            return k;
        }
        id_maker_++;
        string_to_key_.Add(reqCmd, id_maker_);
        return id_maker_;
    }

    public new void OnRevieveMsg(int luaSerial)
    {
        int oldSerial = 0;
        if (new_to_old_serial_.TryGetValue(luaSerial,out oldSerial))
        {
            new_to_old_serial_.Remove(luaSerial);
            int key = 0;
            if (old_serial_to_key_.TryGetValue(oldSerial,out key))
            {
                old_serial_to_key_.Remove(oldSerial);
                key_to_old_serial_.Remove(key);
                base.OnRevieveMsg(oldSerial);
            }
        }
    }
}