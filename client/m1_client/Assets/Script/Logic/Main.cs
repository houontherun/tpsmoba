using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        AppFacade.Instance.StartUp();
        AppFacade.Instance.GetManager<GameManager>(ManagerName.Game).StartGame();
        Application.logMessageReceived += LogHandler;
        LogCenter.Instance().OpenTrace(LogTraceType.File);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (Application.isMobilePlatform)
            Application.targetFrameRate = AppConst.GameFrameRate;
        OnInitialize();
    }
	
	void OnInitialize()
    {
        TableData.Init(delegate() { Debug.Log("loaded DataTable..."); });
    }

    public void LogHandler(string message, string stacktrace, UnityEngine.LogType type)
    {
        StringBuilder s = new StringBuilder();
        s.Append(message);
        s.Append("\r\n");
        if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
        {
            s.Append(stacktrace);
            LogCenter.LogError(s.ToString());
        }
        else if (type == UnityEngine.LogType.Log)
        {
            LogCenter.Log(s.ToString());
        }
        else if (type == UnityEngine.LogType.Warning)
        {
            LogCenter.LogWarning(s.ToString());
        }
    }
}
