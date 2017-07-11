using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.Collections.Generic;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    private string kLoadingBarScene = "";
    public int kSceneIndex = -1;  //当前场景编号
    public string kLoadingTip = "";
    private bool bLoadSceneComplited = false;  // 暂停
    public float kLoadingProgress = 0;
    private bool bLoadResource = false;
    private AsyncOperation kLoadingBarAsyncOperation = null;
    private AsyncOperation kLoadingLevelAsyncOperation = null;
    private AsyncOperation kUnAssetsOperation = null;
    private string kTargetScene = string.Empty;
    private Action kLevelLoadedCallback = null;
    private ResAsyncInfo kLoadConfigResInfo = null;
    AssetBundle bundle = null;
    public void Update()
    {
        HandleLoadingProgress();
    }
    public void LoadScene(string name, Action onFinish = null)
    {
        LoadSceneImpl(name, onFinish);
    }


    internal void UpdateLoadingProgress(float progress)
    {
        kLoadingProgress = progress;
        
    }

    internal void UpdateLoadingTip(string tip)
    {
        kLoadingTip = tip;
    }

    internal void BeginLoading(string name)
    {
        kLoadingProgress = 0;
        kLoadingBarScene = "loading";
        bLoadSceneComplited = false;
        //AppFacade.Instance.SendMessageCommand(NotiConst.LOAD_SCENE_START, name);
    }

   void loadSenenBundle(string name)
    {
        if (AppConst.PublishMode)
        {
            string bundleName = string.Empty;
            if (!name.Contains("loading") && !name.Contains("Login") && !name.Contains("SelectActorScence"))//排除loading login等几个场景
            {
                string ResName = ResDefine.GetResourceType(EResType.eResScene);
                ResName += "/";
                if (name.Contains("_"))
                {
                    int i = name.LastIndexOf('_');
                    bundleName = name.Substring(0, i);
                }
                else
                    bundleName = name;
             //   bundle = AppFacade.Instance.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader).LoadAssetBundle(ResName + bundleName);

            }
        }
    }

    private void LoadSceneImpl(string name, Action onFinish)
    {
        kTargetScene = name;

        BeginLoading(name);
        if (null == kLoadingBarAsyncOperation)
        {
            kLoadingBarAsyncOperation = SceneManager.LoadSceneAsync(kLoadingBarScene);
            kLevelLoadedCallback = onFinish;
        }

    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    IEnumerator LoadData(ResAsyncInfo info)
    {
	    info.Progress = 0.99f;
        yield return new WaitForSeconds(.2f);//强行0.15秒
        TableData.Init(delegate() { info.IsDone = true; });

    }

    
    internal ResAsyncInfo LoadDataByServer()
    {
        ResAsyncInfo info = new ResAsyncInfo();
        info.CurCoroutine = CoroutineInsManager.Instance.StartCoroutine(LoadData(info));

        return info;
    }

    private void HandleLoadingProgress()
    {
        if (bLoadSceneComplited)
        {
            return;
        }
        if (null != kLoadingBarAsyncOperation)
        {
            if (kLoadingBarAsyncOperation.isDone)
            {
                kLoadingBarAsyncOperation = null;
                UpdateLoadingProgress(0.0f);
                ObjectPoolManager.Cleanup();
                UpdateLoadingTip("加载场景不费流量");
                kUnAssetsOperation = Resources.UnloadUnusedAssets();
            }
        }
        else if (kUnAssetsOperation != null)
        {
           if(kUnAssetsOperation.isDone)
           {
               kUnAssetsOperation = null;
               Util.ClearMemory();
               string levelName = kTargetScene;
             //  loadSenenBundle(levelName);
               kLoadingLevelAsyncOperation = SceneManager.LoadSceneAsync(levelName);
           }
           else
           {
               UpdateLoadingProgress(0.0f + kUnAssetsOperation.progress * 0.15f);
           }
        }
        else if (kLoadConfigResInfo != null)
        {
            if (kLoadConfigResInfo.IsDone)
            {
                kLoadConfigResInfo = null;
                bLoadSceneComplited = true;
                UpdateLoadingProgress(1.0f);
 
            }
            else
            {
                UpdateLoadingProgress(kLoadConfigResInfo.Progress);
            }
        }
        else if (null != kLoadingLevelAsyncOperation)//再等待目标场景加载
        {
            if (kLoadingLevelAsyncOperation.isDone)
            {
                kLoadingLevelAsyncOperation = null;
                kLoadConfigResInfo = LoadDataByServer();
                if (null != kLevelLoadedCallback)
                {
                    kLevelLoadedCallback();
                    kLevelLoadedCallback = null;
                }
            }
            else
            {
                UpdateLoadingProgress(0.15f + kLoadingLevelAsyncOperation.progress * 0.8f);
            }
        }
       
    }

    public bool Isloading()
    {
        return (kLoadingProgress > 0 && kLoadingProgress < 0.999f);
    }
}
