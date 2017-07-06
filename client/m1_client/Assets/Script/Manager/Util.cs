using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Util
{
    private static List<string> luaPaths = new List<string>();

    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static float Float(object o)
    {
        return (float)Math.Round(Convert.ToSingle(o), 2);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }
    public static Vector2 RandomVec2(float radius)
    {
        return UnityEngine.Random.insideUnitCircle * radius;
    }
    public static Vector3 RandomVec3(float radius)
    {
        return UnityEngine.Random.insideUnitSphere * radius;
    }
    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static string Uid(string uid)
    {
        int position = uid.LastIndexOf('_');
        return uid.Remove(0, position + 1);
    }

    public static long GetTime()
    {
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)ts.TotalMilliseconds;
    }


    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    public static Component GetComponentInChildren(GameObject go, string type)
    {
        Component c = go.GetComponent(type);
        if (c == null)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                c = GetComponentInChildren(go.transform.GetChild(i).gameObject, type);
                if(c != null)
                {
                    return c;
                }
            }
        }
        return c;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Component版
    /// </summary>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) GameObject.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }
    public static Vector3[] GetCorners(Vector3 start, Vector3 end)
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        if (UnityEngine.AI.NavMesh.CalculatePath(start, end, UnityEngine.AI.NavMesh.AllAreas, path))
        {
            return (path.corners);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(Transform go)
    {
        if (go == null) return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect();

    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
            string game = AppConst.AppName.ToLower();
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            if(Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData).Replace("\\", "/") + "/" + game + "/";
            }
            return Application.dataPath + "/" + AppConst.AssetDir + "/";
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isEditor)
            return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AppConst.AssetDir + "/";
        else
            return "file:///" + DataPath;

    }

    /// <summary>
    /// 取得行文本
    /// </summary>
    public static string GetFileText(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 网络可用
    /// </summary>
    public static bool NetAvailable
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    /// <summary>
    /// 是否是无线
    /// </summary>
    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/" + AppConst.AssetDir + "/";
                break;
        }
        return path;
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string GetWebUrl()
    {
        string WebUrl= string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                WebUrl = AppConst.PathWebUrl + "Android/";
                break;
            case RuntimePlatform.IPhonePlayer:
                WebUrl = AppConst.PathWebUrl + "IOS/";
                break;
            default:
                WebUrl = AppConst.PathWebUrl + "PC/";
                break;
        }
        return WebUrl;
    }


    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string GetPlatformWebUrl()
    {
        string WebUrl = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                WebUrl =  "Android/";
                break;
            case RuntimePlatform.IPhonePlayer:
                WebUrl =  "IOS/";
                break;
            default:
                WebUrl = "PC/";
                break;
        }
        return WebUrl;
    }


    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string GetServerListUrl()
    {
        string WebUrl = string.Empty;
        if (AppConst.PublishMode)
            WebUrl = AppConst.PublishServerListUrl;
        else
            WebUrl = AppConst.DevServerListUrl;
        return WebUrl;
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string GetLocalServerList()
    {
        string LocalUrl = string.Empty;
        if (AppConst.PublishMode)
            LocalUrl = Util.DataPath + AppConst.PublishServerList;
        else
            LocalUrl = Util.DataPath + AppConst.DevServerList;
        return LocalUrl;
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    public static string GetLocalPatchVersion()
    {
        string version = string.Empty;
        if (string.IsNullOrEmpty(AppConst.Version))
        {
            if(File.Exists(Util.DataPath + AppConst.PatchList))
            {
                FilesConfig filelist = JsonHelp.LoadFromJsonFile<FilesConfig>(Util.DataPath + AppConst.PatchList);
                if(filelist!=null)
                {
                    string versionFloder = filelist.VerRevision.GetTxt();
                    version = versionFloder;
                }
                
            }
        }
        else
        {
            version = AppConst.Version;
        }
        return version;
    }


    /// <summary>
    /// 防止初学者不按步骤来操作
    /// </summary>
    /// <returns></returns>
    public static int CheckRuntimeFile()
    {
        if (!Application.isEditor) return 0;
        string streamDir = Application.dataPath + "/StreamingAssets/";
        if (!Directory.Exists(streamDir))
        {
            return -1;
        }
        else if (AppConst.PublishMode)
        {
            string[] files = Directory.GetFiles(streamDir);
            if (files.Length == 0) return -1;

            if (!File.Exists(streamDir + AppConst.PatchList))
            {
                return -1;
            }
        }
        else
        {
            string sourceDir = AppConst.FrameworkRoot + "/ToLua/Generate/";
            if (!Directory.Exists(sourceDir))
            {
                return -2;
            }
            else
            {
                string[] files = Directory.GetFiles(sourceDir);
                if (files.Length == 0) return -2;
            }
            sourceDir = AppConst.FrameworkRoot + "/Lua/";
            string[] pfiles = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
            if (pfiles.Length == 0) return -2;

        }
        return 0;
    }


    public static float GetLoadingProgress()
    {
        return LoadSceneManager.Instance().kLoadingProgress;
    }

    public static void LoadScene(string scenePath, Action func = null)
    {
        LoadSceneManager.Instance().LoadScene(scenePath,  () =>
        {
            if (func != null)
            {
                func();
            }
        });
    }


    // dayofweek表示星期几，hour表示时，min表示分，函数返回现在到指定时间的总秒数
    public static double GetSecondSpan(int dayOfWeek, int hour, int min)
    {
        DateTime weekStart = DateTime.Now.AddDays(0 - Convert.ToDouble(DateTime.Now.DayOfWeek) - 1);
        weekStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day, 0, 0, 0);

        DateTime targetTime = weekStart.AddDays(dayOfWeek).AddHours(hour).AddMinutes(min);
        if(targetTime < DateTime.Now)
        {
            targetTime = targetTime.AddDays(7);
        }
        TimeSpan span = targetTime - DateTime.Now;
        return span.TotalSeconds;
    }

    public static string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }



    public static void Beizer3(GameObject go, float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Transform followTarget = null)
    {
        if(go == null)
        {
            return;
        }
        BezierComp bez = go.GetComponent<BezierComp>();
        if(bez == null)
        {
            bez = go.AddComponent<BezierComp>();
        }
        bez.Bezier3(p0, p1, p2, p3, t);
        if(followTarget != null)
        {
            bez.SetFollowTarget(followTarget);
        }
    }

    public static void Beizer2(GameObject go, float t, Vector3 p0, Vector3 p1, Vector3 p2, Transform followTarget = null)
    {
        if (go == null)
        {
            return;
        }
        BezierComp bez = go.GetComponent<BezierComp>();
        if (bez == null)
        {
            bez = go.AddComponent<BezierComp>();
        }
        bez.Bezier2(p0, p1, p2, t);
        if (followTarget != null)
        {
            bez.SetFollowTarget(followTarget);
        }
    }
}


// ui相关util
public class UIUtil
{
    public static GameObject defaultCanvas = GameObject.Find("Canvas");
    public static void SetUIPosition(Vector3 worldPosition, RectTransform uiRect, float offsetX, float offsetY)
    {
        if (Camera.main == null) return;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        if (screenPosition.z < 0) { screenPosition = Vector3.one * 10000; }
        var cacheCanvas = defaultCanvas.GetComponent<Canvas>();
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cacheCanvas.transform as RectTransform, screenPosition, cacheCanvas.worldCamera, out position);
        uiRect.anchoredPosition3D = new Vector3(position.x + offsetX, position.y + offsetY, 0);
    }


    public static void SetImageFillType(GameObject go, string fillMethod, string origin)
    {
        Image image = go.GetComponent<Image>();
        if (image != null)
        {
            image.type = Image.Type.Filled;
            if (fillMethod == "horizontal")
            {
                image.fillMethod = Image.FillMethod.Horizontal;
                if (origin == "left")           image.fillOrigin = 0; 
                else if (origin == "right")     image.fillOrigin = 1; 
            }
            else if (fillMethod == "vertical")
            {
                image.fillMethod = Image.FillMethod.Vertical;
                if (origin == "bottom")     image.fillOrigin = 0; 
                else if (origin == "top")   image.fillOrigin = 1;
            }
            else if (fillMethod == "radial90")
            {
                image.fillMethod = Image.FillMethod.Radial90;
                if (origin == "bottom_left")     image.fillOrigin = 0;
                else if (origin == "top_left")   image.fillOrigin = 1;
                else if (origin == "top_right") image.fillOrigin = 2;
                else if (origin == "bottom_right") image.fillOrigin = 3;
                image.fillClockwise = false;
            }
            else if (fillMethod == "radial180")
            {
                image.fillMethod = Image.FillMethod.Radial180;
                if (origin == "bottom") image.fillOrigin = 0;
                else if (origin == "left") image.fillOrigin = 1;
                else if (origin == "top") image.fillOrigin = 2;
                else if (origin == "right") image.fillOrigin = 3;
                image.fillClockwise = false;
            }
            else if (fillMethod == "radial360")
            {
                image.fillMethod = Image.FillMethod.Radial360;
                if (origin == "bottom") image.fillOrigin = 0;
                else if (origin == "right") image.fillOrigin = 1;
                else if (origin == "top") image.fillOrigin = 2;
                else if (origin == "left") image.fillOrigin = 3;
                image.fillClockwise = false;
            }
        }
    }

    public static void AddButtonEffect(GameObject go, string pressedSprite = null, string disableSprite = null)
    {
        Button btn = go.GetComponent<Button>();
        if (btn == null)
        {
            btn = go.AddComponent<Button>();
        }
        btn.enabled = true;
        Sprite pres = null, diss = null;
        if (pressedSprite != null)
        {
            pres = ResourceManager.LoadSprite(pressedSprite);
        }
        if(disableSprite != null)
        {
            diss = ResourceManager.LoadSprite(disableSprite);
        }
        if (pres != null || diss != null)
        {
            btn.transition = Selectable.Transition.SpriteSwap;
            SpriteState ss = new SpriteState();
            ss.pressedSprite = pres;
            ss.disabledSprite = diss;
            btn.spriteState = ss;
        }
        else
        {
            btn.transition = Selectable.Transition.ColorTint;
        }
        
    }
    public static void RemoveButtonEffect(GameObject go)
    {
        Button btn = go.GetComponent<Button>();
        if (btn == null)
        {
            return;
        }
        btn.enabled = false;
        //GameObject.DestroyImmediate(btn);
    }

    // 设置锚点
    public static void SetRectAnchor(GameObject go, Vector2 min, Vector2 max)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }
        Vector2 pos = rect.anchoredPosition;

        Vector2 anchorMin = rect.anchorMin;
        Vector2 anchorMax = rect.anchorMax;

        rect.anchorMin = min;
        rect.anchorMax = max;

        if (anchorMin.x == anchorMax.x && anchorMin.y == anchorMax.y &&
            rect.anchorMin.x == rect.anchorMax.x && rect.anchorMin.y == rect.anchorMax.y)
        {
            float wid = 1920;// rect.sizeDelta.x;
            float hei = 1080;// rect.sizeDelta.y;
        
            float dx = rect.anchorMin.x - anchorMin.x;
            float dy = rect.anchorMin.y - anchorMin.y;

            Vector2 adjustPos = new Vector2(pos.x - wid * dx, pos.y - hei * dy);
            rect.anchoredPosition = adjustPos;
        }
    }

    // 设置贴边
    public static void SetRectAttachment(GameObject go, string attach)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }
        SetRectAnchor(go, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        switch (attach)
        {
            //case "left":
            case "l":
                SetRectAnchor(go, new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                break;
            //case "left_bottom":
            case "lb":
                SetRectAnchor(go, new Vector2(0, 0), new Vector2(0, 0));
                break;
            //case "bottom":
            case "b":
                SetRectAnchor(go, new Vector2(0.5f, 0), new Vector2(0.5f, 0));
                break;
            //case "right_bottom":
            case "rb":
                SetRectAnchor(go, new Vector2(1, 0), new Vector2(1, 0));
                break;
            //case "right":
            case "r":
                SetRectAnchor(go, new Vector2(1, 0.5f), new Vector2(1, 0.5f));
                break;
            //case "right_top":
            case "rt":
                SetRectAnchor(go, new Vector2(1, 1), new Vector2(1, 1));
                break;
            //case "top":
            case "t":
                SetRectAnchor(go, new Vector2(0.5f, 1), new Vector2(0.5f, 1));
                break;
            //case "left_top":
            case "lt":
                SetRectAnchor(go, new Vector2(0, 1), new Vector2(0, 1));
                break;
        }
    }
    //public static void SetBatchRectAttachment()
    //{

    //}

    //设置中心点，并重新调整坐标(保持相对位置不变)
    public static void SetRectPivot(GameObject go, Vector2 pivot)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }
        Vector2 pos = rect.anchoredPosition;
        Vector2 piv = rect.pivot;

        rect.pivot = pivot;
        float dx = pivot.x - piv.x;
        float dy = pivot.y - piv.y;

        Vector2 adjustPos = new Vector2(pos.x + rect.sizeDelta.x * dx, pos.y + rect.sizeDelta.y * dy);
        rect.anchoredPosition = adjustPos;
    }

    //增加outline滤镜
    public static void AddTextOutline(GameObject go,Color color)
    {
        Outline outline = go.GetComponent<Outline>();
        if(outline == null)
        {
            outline = go.AddComponent<Outline>();
        }
        outline.effectColor = color;
    }

    //设置文本框居中格式
    public static void SetTextAlignment(Text text,int anchor)
    {
        if(text == null)
        {
            return;
        }
        text.alignment = (TextAnchor)anchor;
    }

    public static Material CreateMaterial(string shaderName)
    {
        Shader shader = Shader.Find(shaderName);
        if(shader == null)
        {
            return null;
        }
        Material material = new Material(shader);
        return material;
    }

    public static void Vibrate()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        UnityEngine.Handheld.Vibrate();
#endif
    }

    public static void AddScrollBarListener(GameObject go, Action<float> onScroll)
	{
		Scrollbar scroll = go.GetComponent<Scrollbar>();
		if(scroll == null)
		{
			return;
		}
		if(onScroll == null)
		{
			return;
		}

		scroll.onValueChanged.AddListener(delegate (float vec) {
			if(onScroll != null)
			{
				onScroll(vec);
			}            
		});
	}
	public static void RemoveAllScrollBarListener(GameObject go)
	{
		Scrollbar scroll = go.GetComponent<Scrollbar>();
		if (scroll == null)
		{
			return;
		}
		scroll.onValueChanged.RemoveAllListeners();
	}


    public static void AddScrollListener(GameObject go, Action<Vector2> onScroll)
    {
        ScrollRect scroll = go.GetComponent<ScrollRect>();
        if(scroll == null)
        {
            return;
        }
        if(onScroll == null)
        {
            return;
        }
        
        scroll.onValueChanged.AddListener(delegate (Vector2 vec) {
            if(onScroll != null)
            {
                onScroll(vec);
            }            
        });
    }
    public static void RemoveAllScrollListener(GameObject go)
    {
        ScrollRect scroll = go.GetComponent<ScrollRect>();
        if (scroll == null)
        {
            return;
        }
        scroll.onValueChanged.RemoveAllListeners();
    }

    static public void SetInputField(InputField inputField, string text)
    {
        if (inputField == null)
        {
            return;
        }

        inputField.text = text;
    }

    public static void AddToggleListener(GameObject go, Action<bool> onValChange)
    {
        Toggle toggle = go.GetComponent<Toggle>();
        if(toggle == null)
        {
            return;
        }
        toggle.onValueChanged.AddListener(delegate(bool b) {
            if(onValChange != null)
            {
                onValChange(b);
            }
        });
    }

    public static void AddSliderListener(GameObject go, Action<float> onValChange)
    {
        Slider slider = go.GetComponent<Slider>();
        if (slider == null)
        {
            return;
        }
        slider.onValueChanged.AddListener(delegate(float b) {
            if(onValChange != null)
            {
                onValChange(b);
            }
        });
    }

    public static void RemoveToggleListener(GameObject go)
    {
        Toggle toggle = go.GetComponent<Toggle>();
        if (toggle == null)
        {
            return;
        }  
        toggle.onValueChanged.RemoveAllListeners();
    }

    public static void AddTMP_InputFieldOnValueChanged(GameObject go, Action<string> onValChange)
    {
        TMPro.TMP_InputField inputField = go.GetComponent<TMPro.TMP_InputField>();
        if (inputField == null)
        {
            return;
        }

        inputField.onValueChanged.AddListener(delegate (string b) {
            if (onValChange != null)
            {
                onValChange(b);
            }
        });
    }

    public static void RemoveTMP_InputFieldOnValueChanged(GameObject go)
    {
        TMPro.TMP_InputField inputField = go.GetComponent<TMPro.TMP_InputField>();
        if (inputField == null)
        {
            return;
        }
        inputField.onValueChanged.RemoveAllListeners();
    }

    public static void AddTMP_InputFieldOnEndEdit(GameObject go, Action<string> onValChange)
    {
        TMPro.TMP_InputField inputField = go.GetComponent<TMPro.TMP_InputField>();
        if (inputField == null)
        {
            return;
        }

        inputField.onEndEdit.AddListener(delegate (string b) {
            if (onValChange != null)
            {
                onValChange(b);
            }
        });
    }

    public static void RemoveTMP_InputFieldOnEndEdit(GameObject go)
    {
        TMPro.TMP_InputField inputField = go.GetComponent<TMPro.TMP_InputField>();
        if (inputField == null)
        {
            return;
        }
        inputField.onEndEdit.RemoveAllListeners();
    }

    public static void SetFullFromParentEdge(RectTransform transform)
    {
        if(transform == null)
        {
            return;
        }
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        transform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
    }


}