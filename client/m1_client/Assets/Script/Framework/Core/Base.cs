using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base : MonoBehaviour {
    private AppFacade kFacade;
    private AssetsLoaderManager kAssetLoadMgr;
    private ResourceManager kResMgr;
    private NetworkManager kNetMgr;
    private TimerManager kTimerMgr;
    private ThreadManager kThreadMgr;
    private ObjectPoolManager kObjectPoolMgr;
    //private ViewManager kViewMgr;

    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RegisterMessage(IView view, List<string> messages) {
        if (messages == null || messages.Count == 0) return;
        Controller.Instance.RegisterViewCommand(view, messages.ToArray());
    }

    /// <summary>
    /// 移除消息
    /// </summary>
    /// <param name="view"></param>
    /// <param name="messages"></param>
    protected void RemoveMessage(IView view, List<string> messages) {
        if (messages == null || messages.Count == 0) return;
        Controller.Instance.RemoveViewCommand(view, messages.ToArray());
    }

    protected AppFacade facade {
        get {
            if (kFacade == null) {
                kFacade = AppFacade.Instance;
            }
            return kFacade;
        }
    }

    protected AssetsLoaderManager AssetLoadManager
    {
        get {
            if (kAssetLoadMgr == null) {
                kAssetLoadMgr = facade.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader);
            }
            return kAssetLoadMgr;
        }
    }

    protected ResourceManager ResManager
    {
        get
        {
            if (kResMgr == null)
            {
                kResMgr = facade.GetManager<ResourceManager>(ManagerName.Resource);
            }
            return kResMgr;
        }
    }

    protected NetworkManager NetManager {
        get {
            if (kNetMgr == null) {
                kNetMgr = facade.GetManager<NetworkManager>(ManagerName.Network);
            }
            return kNetMgr;
        }
    }

    protected TimerManager TimerManager {
        get {
            if (kTimerMgr == null) {
                kTimerMgr = facade.GetManager<TimerManager>(ManagerName.Timer);
            }
            return kTimerMgr;
        }
    }

    protected ThreadManager ThreadManager {
        get {
            if (kThreadMgr == null) {
                kThreadMgr = facade.GetManager<ThreadManager>(ManagerName.Thread);
            }
            return kThreadMgr;
        }
    }

    protected ObjectPoolManager ObjPoolManager {
        get {
            if (kObjectPoolMgr == null) {
                kObjectPoolMgr = facade.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
            }
            return kObjectPoolMgr;
        }
    }
    //protected ViewManager ViewMgr
    //{
    //    get
    //    {
    //        if (kViewMgr == null)
    //        {
    //            kViewMgr = facade.GetManager<ViewManager>(ManagerName.View);
    //        }
    //        return kViewMgr;
    //    }
    //}
}
