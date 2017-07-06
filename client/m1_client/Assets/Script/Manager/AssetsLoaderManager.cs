using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UObject = UnityEngine.Object;
public class ResAsyncInfo
{
    public float Progress = 0;
    public bool IsDone = false;
    public bool IsError = false;
    public string Tip = string.Empty;
    public Coroutine CurCoroutine = null;
    public System.Object Target = null;
}

public class AssetBundleInfo {
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public AssetBundleInfo(AssetBundle assetBundle) {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}
public class AssetsLoaderManager : Manager {
        string m_BaseDownloadingURL = "";
        string[] m_AllManifest = null;
        AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

        class LoadAssetRequest {
            public Type assetType;
            public string[] assetNames;
            public Action<UObject[]> sharpFunc;
        }

        // Load AssetBundleManifest.
        public void Initialize(string manifestName, Action initOK) {
            m_BaseDownloadingURL = Util.GetRelativePath();
            LoadAsset<AssetBundleManifest>(manifestName, new string[] { "AssetBundleManifest" }, delegate(UObject[] objs) {
                if (objs.Length > 0) {
                    m_AssetBundleManifest = objs[0] as AssetBundleManifest;
                    m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                }
                if (initOK != null) initOK();
            });
        }

        public void LoadPrefab(string abName, string assetName, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, new string[] { assetName }, func);
        }

        public void LoadPrefab(string abName, string[] assetNames, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, assetNames, func);
        }


       public void LoadAsset(string abName, string assetName, Action<UObject[]> func) {
            LoadAsset<UnityEngine.Object>(abName, new string[] { assetName }, func);
        }

       public void LoadRes(string abPath, Action<UObject> func)
       {
           StartCoroutine(loadResAsync(abPath,func));
       }

       public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object
       {
           abname = abname.ToLower();
           AssetBundle bundle = LoadAssetBundle(abname);
           if (bundle == null) return null;
           T obj = bundle.LoadAsset<T>(assetname);
           return obj;

       }
       /// <summary>
       /// 载入AssetBundle
       /// </summary>
       /// <param name="abname"></param>
       /// <returns></returns>
       public AssetBundle LoadAssetBundle(string abname)
       {
           abname = abname.ToLower();
           if (!abname.EndsWith(ResDefine.ExtName))
           {
               abname += ResDefine.ExtName;
           }
           AssetBundleInfo bundle = null;
           if (m_AssetBundleManifest == null)
           {
               return bundle.m_AssetBundle;
           }
           if (!m_LoadedAssetBundles.ContainsKey(abname))
           {
               byte[] stream = null;
               string uri = Util.DataPath + abname;
               if (!File.Exists(uri))
               {
                   Debug.LogError(uri + " not exist ");
                   return null;
               }

               LoadDependencies(abname);

               stream = File.ReadAllBytes(uri);
               if (stream.Length == 0)
               {
                   Debug.LogError("load error " + abname);
                   return null;
               }
               byte[] assetBytes = ResDefine.DecryptAssetbundle(stream);
               AssetBundle bundleObj = AssetBundle.LoadFromMemory(assetBytes); 
              if (bundleObj != null)
               {
                   m_LoadedAssetBundles.Add(abname, new AssetBundleInfo(bundleObj));
               }
              return bundleObj;
           }
           else
           {
               m_LoadedAssetBundles.TryGetValue(abname, out bundle);
               return bundle.m_AssetBundle;
           }
           
       }

       /// <summary>
       /// 载入依赖
       /// </summary>
       /// <param name="name"></param>
       void LoadDependencies(string name)
       {
           if (m_AssetBundleManifest == null)
           {
               // Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
               return;
           }
           string[] dependencies = m_AssetBundleManifest.GetAllDependencies(name);
           if (dependencies.Length == 0) return;

           for (int i = 0; i < dependencies.Length; i++)
           {
               LoadAssetBundle(dependencies[i]);
           }
       }

        string GetRealAssetPath(string abName) {
            if (abName.Equals(AppConst.AssetDir)) {
                return abName;
            }
            abName = abName.ToLower();
            if (!abName.EndsWith( ResDefine.ExtName)) {
                abName += ResDefine.ExtName;
            }
            if (abName.Contains("/")) {
                return abName;
            }
            //string[] paths = m_AssetBundleManifest.GetAllAssetBundles();  产生GC，需要缓存结果
            for (int i = 0; i < m_AllManifest.Length; i++) {
                int index = m_AllManifest[i].LastIndexOf('/');  
                string path = m_AllManifest[i].Remove(0, index + 1);    //字符串操作函数都会产生GC
                if (path.Equals(abName)) {
                    return m_AllManifest[i];
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        void LoadAsset<T>(string abName, string[] assetNames, Action<UObject[]> action = null) where T : UObject {
            abName = GetRealAssetPath(abName);

            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetNames = assetNames;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(abName, out requests)) {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);
                StartCoroutine(OnLoadAsset<T>(abName));
            } else {
                requests.Add(request);
            }
        }

        IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            
            if (bundleInfo == null) {
                yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));

                bundleInfo = GetLoadedAssetBundle(abName);

                if (bundleInfo == null) {
                    m_LoadRequests.Remove(abName);
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list)) {
                m_LoadRequests.Remove(abName);
                yield break;
            }
            for (int i = 0; i < list.Count; i++) {
                string[] assetNames = list[i].assetNames;
                List<UObject> result = new List<UObject>();

                AssetBundle ab = bundleInfo.m_AssetBundle;
                for (int j = 0; j < assetNames.Length; j++) {
                    string assetPath = assetNames[j];
                    AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                    
                    yield return request;
                    result.Add(request.asset);
                }

                if (list[i].sharpFunc != null) {
                    list[i].sharpFunc(result.ToArray());
                    list[i].sharpFunc = null;
                }
                //bundleInfo.m_ReferencedCount++;
            }
            if(m_LoadRequests.ContainsKey(abName))
               m_LoadRequests.Remove(abName);
            int RandomSecond = UnityEngine.Random.Range(10, 20);//随机一个时间卸载 避免同时卸载造成压力
            yield return new WaitForSeconds(RandomSecond);
            UnloadAssetBundle(abName);
        }

        IEnumerator OnLoadAssetBundle(string abName, Type type) {
            string url = Util.GetRelativePath() +abName;
             //AssetBundleCreateRequest asset = null;
            WWW download = null;
            if (type == typeof(AssetBundleManifest))
            {
                download = new WWW(url);
            }
             else {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0) {
                    if (!m_Dependencies.ContainsKey(abName))
                       m_Dependencies.Add(abName, dependencies);
                    for (int i = 0; i < dependencies.Length; i++) {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo)) {
                            bundleInfo.m_ReferencedCount++;
                        } else if (!m_LoadRequests.ContainsKey(depName)) {
                            yield return StartCoroutine(OnLoadAssetBundle(depName, type));

                        }
                    }
                }
                if (m_LoadedAssetBundles.ContainsKey(abName))
                {
                    yield break;
                }
                    
               download = new WWW(url);
               download.threadPriority = ThreadPriority.BelowNormal;
            }
            yield return download;
            if (download.error != null)
            {
                Debug.LogError(url);
                Debug.LogError(download.error);
                yield break;
            }
            AssetBundleInfo kbundleInfo = null;
            if (m_LoadedAssetBundles.TryGetValue(abName, out kbundleInfo))
            {
                kbundleInfo.m_ReferencedCount++;
            }
            else
            {
                if (download.bytes.Length == 0)
                {
                    Debug.LogError("load error " + abName);
                }
                byte[] assetBytes = ResDefine.DecryptAssetbundle(download.bytes);
                AssetBundle assetObj = AssetBundle.LoadFromMemory(assetBytes);
                if (assetObj != null)
                {
                    m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
                }
            }
            download.Dispose();
            download = null;
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            string[] dependencies = null;


            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies) {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        /// <summary>
        /// 卸载bundle
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false) {
            abName = GetRealAssetPath(abName);
            //Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            bool bUnload = UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough) {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies) {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        bool UnloadAssetBundleInternal(string abName, bool isThorough) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return false;

            if (--bundle.m_ReferencedCount < 0) {
                if (m_LoadRequests.ContainsKey(abName)) {
                    return false;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                //Debug.Log(abName + " has been unloaded successfully");
                return true;
            }
            return false;
        }

       public void UnloadSyncAssetBundle(string abName)
        {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return;

            bundle.m_AssetBundle.Unload(false);
            m_LoadedAssetBundles.Remove(abName);

        }

        private IEnumerator loadResAsync(string abPath, Action<UObject> whenDone)
        {
            UnityEngine.Object obj = null;
            var resourceRequest = Resources.LoadAsync(abPath);
            yield return resourceRequest;
            obj = resourceRequest.asset;
            if (obj == null)
                Debug.LogWarning("Impossible to find prefab '" + abPath + "'");
            else
              whenDone(obj);
        }

}


#if SYNC_MODE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UObject = UnityEngine.Object;

public class ResAsyncInfo
{
    public float Progress = 0;
    public bool IsDone = false;
    public bool IsError = false;
    public string Tip = string.Empty;
    public Coroutine CurCoroutine = null;
    public System.Object Target = null;
}

public class AssetsLoaderManager : Manager
    {
        private string[] m_Variants = { };
        private AssetBundleManifest manifest;
        private AssetBundle shared, assetbundle;
        private Dictionary<string, AssetBundle> bundles;
       
       void Awake()
        {

           bundles = new Dictionary<string, AssetBundle>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            byte[] stream = null;
            string uri = string.Empty;
            
            uri = Util.DataPath + AppConst.AssetDir;
            if (!File.Exists(uri)) return;
            stream = File.ReadAllBytes(uri);
            ResDefine.DecryptAssetbundle(stream);
            assetbundle = AssetBundle.LoadFromMemory(stream);
            manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object
        {
            abname = abname.ToLower();
            AssetBundle bundle = LoadAssetBundle(abname);
            if (bundle == null) return null;
		    T obj = bundle.LoadAsset<T>(assetname);
		   if(abname.EndsWith("fontassets/fonts & materials.unity3d"))
	 	   {
			  T[] objs = bundle.LoadAllAssets<T>();
			   print(objs[0].name);
		   }
		  //  bundle.Unload(false);
		    return obj;
            //return bundle.LoadAsset<T>(assetname);
        }

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func)
        {
            abName = abName.ToLower();
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++)
            {
                UObject go = LoadAsset<UObject>(abName, assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func.Call((object)result.ToArray());
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abname"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abname)
        {
           abname = abname.ToLower();
           if (!abname.EndsWith(ResDefine.ExtName))
            {
                abname += ResDefine.ExtName;
            }
            AssetBundle bundle = null;
            if (manifest == null)
            {
                return bundle;
            }
            if (!bundles.ContainsKey(abname))
            {
                byte[] stream = null;
                string uri = Util.DataPath + abname;
                if(!File.Exists(uri))
                {
                   Debug.LogError(uri + " not exist ");
                   return bundle;
                }
               
                LoadDependencies(abname);
                
                stream = File.ReadAllBytes(uri);
               if (stream.Length == 0)
               {
                  Debug.LogError("load error " + abname );
                  return bundle;
               } 
                ResDefine.DecryptAssetbundle(stream);
                bundle = AssetBundle.LoadFromMemory(stream); //关联数据的素材绑定
                bundles.Add(abname, bundle);
            }
            else
            {
                bundles.TryGetValue(abname, out bundle);
            }
            return bundle;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string name)
        {
            if (manifest == null)
            {
               // Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = manifest.GetAllDependencies(name);
            if (dependencies.Length == 0) return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundle(dependencies[i]);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();

            // If the asset bundle doesn't have variant, simply return.
            if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
                return assetBundleName;

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_Variants, curSplit[1]);
                if (found != -1 && found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFitIndex != -1)
                return bundlesWithVariant[bestFitIndex];
            else
                return assetBundleName;
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        void OnDestroy()
        {
            if (shared != null) shared.Unload(true);
            if (manifest != null) manifest = null;
            Debug.Log("~AssetsLoaderManager was destroy!");
        }
    }
#endif

