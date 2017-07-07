/********************************************************************************
** auth： yanwei
** date： 2016-08-08
** desc： 资源管理器，提供资源缓存重用机制。
*********************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class ObjectPoolManager : Manager
{
    public void PreloadResource(string res, int count, EResType resTpye)
    {
        ObjectPoolSystem.Instance.PreloadResource(res, count, resTpye);
    }

    public static void PreloadResource(UnityEngine.Object prefab, int count)
    {
        ObjectPoolSystem.Instance.PreloadResource(prefab, count);
    }

    public static void PreloadSharedResource(string res, EResType restype)
    {
        ObjectPoolSystem.Instance.PreloadSharedResource(res, restype);
    }

    public static void NewObject(string res, EResType resType, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolSystem.Instance.NewObject(res, resType, func);
    }

    public static void NewObject(string res, EResType resType, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolSystem.Instance.NewObject(res, resType, timeToRecycle, 120, func);
    }

    public static void NewObject(string res, EResType resType, float timeToRecycle, float timeToDestroy,Action<UnityEngine.Object> func = null)
    {
        ObjectPoolSystem.Instance.NewObject(res, resType, timeToRecycle, timeToDestroy, func);
    }

    public static UnityEngine.Object NewObject(UnityEngine.Object prefab)
    {
        return ObjectPoolSystem.Instance.NewObject(prefab);
    }
    public static UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle)
    {
        return ObjectPoolSystem.Instance.NewObject(prefab, timeToRecycle, 0);
    }

    public static bool RecycleObject(UnityEngine.Object obj)
    {
        return ObjectPoolSystem.Instance.RecycleObject(obj);
    }

    public static UnityEngine.Object GetSharedResource(string res, EResType restype, Action<UnityEngine.Object[]> func = null)
    {
        return ObjectPoolSystem.Instance.GetSharedResource(res, restype, func);
    }

    public static void Cleanup()
    {
        ObjectPoolSystem.Instance.CleanupResourcePool();
    }

    public static void Tick()
    {
        ObjectPoolSystem.Instance.Tick();
    }
}

public class ObjectPoolSystem
{
    private GameObject goParent = null;
    private ObjectPool<GameObjectPool> kUsedResourceInfoPool = new ObjectPool<GameObjectPool>();

    private HashSet<string> kPreloadResources = new HashSet<string>();
    private Dictionary<string, UnityEngine.Object> kLoadedPrefabs = new Dictionary<string, UnityEngine.Object>();
    private List<string> kWaitDeleteLoadedPrefabEntrys = new List<string>();
    private SortedList<int, GameObjectPool> kDestoryResources = new SortedList<int, GameObjectPool>();
    private LinkedListDictionary<int, GameObjectPool> kUsedResources = new LinkedListDictionary<int, GameObjectPool>();
    private Dictionary<string, Queue<UnityEngine.Object>> kUnusedResources = new Dictionary<string, Queue<UnityEngine.Object>>();
    private float kLastTickTime = 0;
    public static ObjectPoolSystem Instance
    {
        get { return s_Instance; }
    }
    private static ObjectPoolSystem s_Instance = new ObjectPoolSystem();

    private class GameObjectPool : IPoolAllocatedObject<GameObjectPool>
    {
        internal int kObjId;
        internal UnityEngine.Object kObject;
        internal string kRes;
        internal float kRecycleTime;
        internal float kDestoryTime = 0f;

        internal void Recycle()
        {
            kObject = null;
            kPool.Recycle(this);
        }
        public void InitPool(ObjectPool<GameObjectPool> pool)
        {
            kPool = pool;
        }
        public GameObjectPool Downcast()
        {
            return this;
        }
        private ObjectPool<GameObjectPool> kPool = null;
    }


    internal void PreloadResource(string res, int count, EResType restype)
    {
        UnityEngine.Object prefab = GetSharedResource(res, restype);
        PreloadResource(prefab, count);
    }
    internal void PreloadResource(UnityEngine.Object prefab, int count)
    {
        if (null != prefab)
        {
            //if (!kPreloadResources.Contains(prefab.GetInstanceID()))
            //    kPreloadResources.Add(prefab.GetInstanceID());
            for (int i = 0; i < count; ++i)
            {
                UnityEngine.Object obj = GameObject.Instantiate(prefab);
               // AddToUnusedResources(prefab.GetInstanceID(), obj);
            }
        }
    }

    internal void PreloadSharedResource(string res, EResType restype)
    {
        UnityEngine.Object prefab = GetSharedResource(res, restype);
        if (null != prefab)
        {
            if (!kPreloadResources.Contains(res))
                kPreloadResources.Add(res);
        }
    }
    internal void NewObject(string res, EResType resType, Action<UnityEngine.Object> func = null)
    {
        NewObject(res, resType, 0,60, func);
    }
    internal void NewObject(string res, EResType resType, float timeToRecycle, float timeToDestory, Action<UnityEngine.Object> func = null)
    {
        UnityEngine.Object obj = null;
        UnityEngine.Object retObj = null;
        string abName = string.Empty;
        string resName = string.Empty;

        if (kLoadedPrefabs.ContainsKey(res) && kLoadedPrefabs[res] != null)
        {
            obj = kLoadedPrefabs[res];
            if (obj != null)
            {
                retObj = NewObject(res,obj, timeToRecycle, timeToDestory);
                if (func != null) func(retObj);
            }
        }
        else
        {
            string ResName = ResDefine.GetResourceType(resType);
            string ResFolderName = ResName + "/";
            string abPath = string.Empty;
            if (AppConst.PublishMode && resType != EResType.eSceneLoadRes)
            {
                abName = string.Empty;
                abPath = Path.GetDirectoryName(res);
                if (string.IsNullOrEmpty(abPath))
                {
                    abName = ResFolderName + ResName + ResDefine.ExtName;
                }
                else
                {
                    abPath = abPath.Replace("/", "_");
                    abName = ResFolderName + abPath + ResDefine.ExtName;
                }
                resName = Path.GetFileName(res);
                AppFacade.Instance.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader).LoadAsset(abName, resName, delegate(UnityEngine.Object[] uObj)
                {
                    if (uObj == null || uObj[0] == null)
                    {
                        UnityEngine.Debug.LogWarning("LoadAsset failed abpath:" + abName);
                        UnityEngine.Debug.LogWarning("LoadAsset failed abName:" + resName);
                    }
                    else
                    {
                        retObj = NewObject(res,uObj[0], timeToRecycle, timeToDestory);
                           if (func != null) func(retObj);
                           if (!kLoadedPrefabs.ContainsKey(res))
                               kLoadedPrefabs.Add(res, retObj);
                    }
                }
                );
            }
            else
            {
                if (obj == null)
                {
                    abPath = ResFolderName + res;
                   // obj = Resources.Load(abPath);
                    AppFacade.Instance.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader).LoadRes(abPath, delegate(UnityEngine.Object uObj)
                    {
                        if (uObj == null)
                        {
                            UnityEngine.Debug.LogWarning("LoadAsset failed abpath:" + abName);
                            UnityEngine.Debug.LogWarning("LoadAsset failed abName:" + resName);
                        }
                        retObj = NewObject(res, uObj, timeToRecycle, timeToDestory);
                        if (func != null) func(retObj);
                        if (!kLoadedPrefabs.ContainsKey(res))
                            kLoadedPrefabs.Add(res, retObj);
                    });
                   
                }
               
            }

        }

    }

    internal UnityEngine.Object NewObject(UnityEngine.Object prefab)
    {
        return NewObject(prefab, 0, 0);
    }

    internal UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle, float timeToDestory)
    {
        UnityEngine.Object obj = null;
        if (null != prefab)
        {
            obj = NewObject(prefab.name, prefab,timeToRecycle, timeToDestory);
        }
        return obj;
    }
    internal UnityEngine.Object NewObject(string name, UnityEngine.Object prefab,float timeToRecycle, float timeToDestory)
    {
        UnityEngine.Object obj = null;
        if (!string.IsNullOrEmpty(name))
        {
            float curTime = Time.time;
            float time = timeToRecycle;
            if (timeToRecycle > 0)
                time += curTime;
            float destorytime = timeToDestory;
            if (timeToDestory > 0)
                destorytime += curTime;
            obj = NewFromUnusedResources(name);
            if (null == obj)
            {
                obj = UnityEngine.Object.Instantiate(prefab);
            }
            if (null != obj)
            {
                AddToUsedResources(obj, name, time, destorytime);

                InitializeObject(obj);
            }
        }
        return obj;
    }
    internal bool RecycleObject(UnityEngine.Object obj)
    {
        bool ret = false;
        if (null != obj)
        {
            UnityEngine.GameObject gameObject = obj as UnityEngine.GameObject;
            if (null != gameObject)
            {
                //LogicSystem.LogicLog("RecycleObject {0} {1}", gameObject.name, gameObject.tag);
            }

            int objId = obj.GetInstanceID();
            if (kUsedResources.Contains(objId))
            {
                GameObjectPool resInfo = kUsedResources[objId];
                if (null != resInfo)
                {
                    FinalizeObject(resInfo.kObject);
                    RemoveFromUsedResources(objId);
                    AddToUnusedResources(resInfo.kRes, obj);
                    resInfo.Recycle();
                    if (kDestoryResources.ContainsKey(objId))
                        kDestoryResources[objId] = resInfo;
                    else
                        kDestoryResources.Add(objId, resInfo);
                    ret = true;
                }
            }
        }
        return ret;
    }
    internal void Tick()
    {
        float curTime = Time.time;

        for (LinkedListNode<GameObjectPool> node = kUsedResources.FirstValue; null != node; )
        {
            GameObjectPool resInfo = node.Value;
            if (resInfo.kRecycleTime > 0 && resInfo.kRecycleTime < curTime)
            {
                node = node.Next;

                UnityEngine.GameObject gameObject = resInfo.kObject as UnityEngine.GameObject;
                if (null != gameObject)
                {
                    //LogicSystem.LogicLog("RecycleObject {0} {1} by Tick", gameObject.name, gameObject.tag);
                }

                FinalizeObject(resInfo.kObject);
                AddToUnusedResources(resInfo.kRes, resInfo.kObject);
                RemoveFromUsedResources(resInfo.kObjId);
                resInfo.Recycle();
                if (kDestoryResources.ContainsKey(resInfo.kObjId))
                    kDestoryResources[resInfo.kObjId] = resInfo;
                else
                    kDestoryResources.Add(resInfo.kObjId, resInfo);
            }
            else
            {
                node = node.Next;
            }
          
        }
        if (kDestoryResources.Count < 1) return;
        IList<int> keys = kDestoryResources.Keys;
        for (int i = 0; i < keys.Count; i++)
        {
            if (kDestoryResources[keys[i]].kDestoryTime > 0 && kDestoryResources[keys[i]].kDestoryTime < curTime)
            {
                DeleteFromUnUsedResources(kDestoryResources[keys[i]]);
                kDestoryResources.Remove(keys[i]);
            }
        }
    }

    internal UnityEngine.Object GetSharedResource(string res, EResType ResGroup, Action<UnityEngine.Object[]> func = null)
    {
        UnityEngine.Object obj = null;
        string abName = string.Empty;
        string resName = string.Empty;
        UnityEngine.Object retObj = null;
        if (string.IsNullOrEmpty(res))
        {
            return obj;
        }
        if (kLoadedPrefabs.ContainsKey(res))
        {
            retObj = kLoadedPrefabs[res];
        }
        else
        {
            string ResName = ResDefine.GetResourceType(ResGroup);
            string ResFolderName = ResName + "/";
            string abPath = string.Empty;
            if (AppConst.PublishMode)
            {
                abName = string.Empty;
                abPath = Path.GetDirectoryName(res);
                if (string.IsNullOrEmpty(abPath))
                {
                    abName = ResFolderName + ResName + ResDefine.ExtName;
                }
                else
                {
                    abPath = abPath.Replace("/", "_");
                    abName = ResFolderName + abPath + ResDefine.ExtName;
                }
                resName = Path.GetFileName(res);
                obj = AppFacade.Instance.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader).LoadAsset<UnityEngine.Object>(abName, resName);
            }
            if (obj == null)
            {
                abPath = ResFolderName + res;
                obj = Resources.Load(abPath);
            }
            if (obj != null)
            {

                retObj = UnityEngine.Object.Instantiate(obj);

                kLoadedPrefabs.Add(res, retObj);
               AssetsLoaderManager assetLoader =  AppFacade.Instance.GetManager<AssetsLoaderManager>(ManagerName.AssetsLoader);
               if (assetLoader != null)
                   assetLoader.UnloadSyncAssetBundle(abName);
            }
            else
            {
                UnityEngine.Debug.LogWarning("LoadAsset failed abpath:" + abName);
                UnityEngine.Debug.LogWarning("LoadAsset failed abName:" + resName);
            }
        }
        return retObj;
    }

    internal void CleanupResourcePool()
    {
        for (LinkedListNode<GameObjectPool> node = kUsedResources.FirstValue; null != node; )
        {
            GameObjectPool resInfo = node.Value;
            if (!kPreloadResources.Contains(resInfo.kRes))
            {
                node = node.Next;
                RemoveFromUsedResources(resInfo.kObjId);
                resInfo.Recycle();
                if(resInfo.kObject)
                {
                    GameObject.Destroy(resInfo.kObject);
                }
            }
            else
            {
                node = node.Next;
            }
        }

        foreach (string key in kUnusedResources.Keys)
        {
            if (kPreloadResources.Contains(key))
                continue;
            Queue<UnityEngine.Object> queue = kUnusedResources[key];
            queue.Clear();
        }

        foreach (string key in kLoadedPrefabs.Keys)
        {
            UnityEngine.Object obj = kLoadedPrefabs[key];
            if (null != obj)
            {
                try
                {
                    if (!kPreloadResources.Contains(key))
                    {
                        kWaitDeleteLoadedPrefabEntrys.Add(key);
                    }
                }
                catch (Exception ex)
                {
                    kWaitDeleteLoadedPrefabEntrys.Add(key);
                }
            }
            else
            {
                kWaitDeleteLoadedPrefabEntrys.Add(key);
            }
            obj = null;
        }
        foreach (string key in kWaitDeleteLoadedPrefabEntrys)
        {
            kLoadedPrefabs.Remove(key);
        }
        kWaitDeleteLoadedPrefabEntrys.Clear();
        kUnusedResources.Clear();
        kDestoryResources.Clear();
    }

    private UnityEngine.Object NewFromUnusedResources(string resname)
    {
        UnityEngine.Object obj = null;
        if (kUnusedResources.ContainsKey(resname))
        {
            Queue<UnityEngine.Object> queue = kUnusedResources[resname];
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
                int objId = obj.GetInstanceID();
                if (kDestoryResources.ContainsKey(objId))
                {
                    kDestoryResources.Remove(objId);
                }
            }
            
        }
        return obj;
    }
    private void AddToUnusedResources(string res, UnityEngine.Object obj)
    {
        if (kUnusedResources.ContainsKey(res))
        {
            Queue<UnityEngine.Object> queue = kUnusedResources[res];
            queue.Enqueue(obj);
        }
        else
        {
            Queue<UnityEngine.Object> queue = new Queue<UnityEngine.Object>();
            queue.Enqueue(obj);
            kUnusedResources.Add(res, queue);
        }
    }

    private void AddToUsedResources(UnityEngine.Object obj, string res, float recycleTime, float destoryTime)
    {
        int objId = obj.GetInstanceID();
        if (!kUsedResources.Contains(objId))
        {
            GameObjectPool info = kUsedResourceInfoPool.Alloc();
            info.kObjId = objId;
            info.kObject = obj;
            info.kRes = res;
            info.kRecycleTime = recycleTime;
            info.kDestoryTime = destoryTime;
            kUsedResources.AddLast(objId, info);
        }
    }
    private void RemoveFromUsedResources(int objId)
    {
        kUsedResources.Remove(objId);
    }

    private void DeleteFromUnUsedResources(GameObjectPool goPool)
    {
        if (kUnusedResources.ContainsKey(goPool.kRes))
        {
            Queue<UnityEngine.Object> queue = kUnusedResources[goPool.kRes];
            if(queue.Count >0)
            {
                 UnityEngine.Object obj = queue.Dequeue();
                 GameObject.Destroy(obj);
            }
            if(queue.Count < 1)
            {
                kUnusedResources.Remove(goPool.kRes);
                kLoadedPrefabs.Remove(goPool.kRes);
            }
            
        }
    }

    private void InitializeObject(UnityEngine.Object obj)
    {
        GameObject gameObj = obj as GameObject;
        if (null != gameObj)
        {
            if (!gameObj.activeSelf)
                gameObj.SetActive(true);
            /*ParticleSystem[] pss = gameObj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in pss) {
              if (null != ps && ps.playOnAwake) {
                ps.Play();
              }
            }*/
            ParticleSystem ps = gameObj.GetComponent<ParticleSystem>();
            if (null != ps && ps.playOnAwake)
            {
                ps.Play();
            }
        }
    }
    private void FinalizeObject(UnityEngine.Object obj)
    {
        if (goParent == null)
        {
            goParent = new GameObject();
            goParent.name = "ObjectPoolParent";
        }
        GameObject gameObj = obj as GameObject;
        if (null != gameObj)
        {
            ParticleSystem ps0 = gameObj.GetComponent<ParticleSystem>();
            if (null != ps0 && ps0.playOnAwake)
            {
                ps0.Stop();
            }
            ParticleSystem[] pss = gameObj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in pss)
            {
                if (null != ps)
                {
                    ps.Clear();
                }
            }
            gameObj.transform.SetParent(goParent.transform, false);
            if (gameObj.activeSelf)
                gameObj.SetActive(false);
        }
    }

    //得到唯一标识码
    internal int GenToInt32(params byte[] v)
    {
        var r = 0;
        var len = v.Length;
        byte[] intByte;

        if (len > 4)
        {
            intByte = new byte[4];
            intByte[0] =(byte)( v[0]^ v[(int)(len / 2 + 1)]);
            intByte[1] = v[(int)(len / 2)];
            intByte[2] = (byte)(v[len - 2]&v[len - 1]);
            intByte[3] = (byte)(v[len - 1] | len);
            for (int i = 0; i < len; i++)
            {
                intByte[2] ^= (byte)(v[i]);
                intByte[3] ^= (byte)(v[i]);
            }

            len = 4;
        }
        else
        {
            intByte = new byte[len];
            for (var i = 0; i < len; i++)
            {
                intByte[i] = v[i];
            }
        }

        for (var i = 0; i < len; i++)
        {
            r |= intByte[i] << 8 * (len - i - 1);
        }
        return r;
    }


    private ObjectPoolSystem()
    {
        kUsedResourceInfoPool.Init(256);
    }

}



