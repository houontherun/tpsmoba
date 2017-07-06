using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AppFacade : Facade
{
    private static AppFacade _instance;

    private GameManager _gameManager;
    public GameManager gameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = GetManager<GameManager>(ManagerName.Game);
            }
            return _gameManager;
        }
    }

    private TimerManager _timeManager;
    public TimerManager timerManager
    {
        get
        {
            if (_timeManager == null)
            {
                _timeManager = GetManager<TimerManager>(ManagerName.Timer);
            }
            return _timeManager;
        }
    }

    private NetworkManager _networkManager;
    public NetworkManager networkManager
    {
        get {
            if (_networkManager == null)
            {
                _networkManager = GetManager<NetworkManager>(ManagerName.Network);
            }
            return _networkManager;
        }
    }

    public AppFacade() : base()
    {
    }

    public static AppFacade Instance
    {
        get{
            if (_instance == null) {
                _instance = new AppFacade();
            }
            return _instance;
        }
    }

    override protected void InitFramework()
    {
        base.InitFramework();
        RegisterCommand(NotiConst.START_UP, typeof(StartUpCommand));
    }

    /// <summary>
    /// 启动框架
    /// </summary>
    public void StartUp() {
        SendMessageCommand(NotiConst.START_UP);
        RemoveMultiCommand(NotiConst.START_UP);
    }

    
}

