using UnityEngine;

public interface INetMessageHandler
{
    void Handle(Connection con, int action, byte[] data);
}

public class NetworkManager : Manager
{
    // 主服务器
    private Connection mainConn = new Connection("Main");
    public Connection MainConnection
    {
        get
        {
            return mainConn;
        }
    }

    // 战斗服务器
    private Connection fightConn = new Connection("Fight");
    public Connection FightConnection
    {
        get
        {
            return fightConn;
        }
    }

    void OnMainStateChange(ConnectState state, string message)
    {
       // Util.CallMethod("ConnectionManager", "OnStateChanged", mainConn, state, message);
    }
    void OnFightStateChange(ConnectState state, string message)
    {
       // Util.CallMethod("ConnectionManager", "OnStateChanged", fightConn, state, message);
    }

    void Start()
    {
        mainConn.OnStateChanged = OnMainStateChange;
        fightConn.OnStateChanged = OnFightStateChange;
    }
    void Update()
    {
        mainConn.Update();
        fightConn.Update();
    }
    public void Close()
    {
        MainConnection.Close();
        FightConnection.Close();
    }

    public Connection GetConnection()
    {
        if(fightConn.state == ConnectState.Success)
        {
            return fightConn;
        }
        return mainConn;
    }
}
