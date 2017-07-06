using UnityEngine;
using System.Collections;
public class StartUpCommand : ControllerCommand {

    public override void Execute(IMessage message) {

        //-----------------关联命令-----------------------
        AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE, typeof(SocketCommand));

        //-----------------初始化管理器-----------------------
        AppFacade.Instance.AddManager<AssetsLoaderManager>(ManagerName.AssetsLoader);
        AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
        //AppFacade.Instance.AddManager<ViewManager>(ManagerName.View);
        AppFacade.Instance.AddManager<TimerManager>(ManagerName.Timer);
        AppFacade.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);
        AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
        AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
    }

}