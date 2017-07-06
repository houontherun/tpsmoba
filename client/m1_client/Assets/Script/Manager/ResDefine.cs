using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

//Res的类型
public enum EResType
{
    eResCommon = 0,      //Common Res
    eResPicture,         //icon跟场景背景的Res
    eResAudio,           //音乐
    eResCharacter,       //角色
    eResMonster,         //怪物bundle
    eResBoss,            //Boss 
    eResPet,			 //宠物模型
    eResScene,           //场景的 Res
    eResUI,              //UI Res
    eResUIEffect,        //UI 特效
    eResModel,           //模型文件（包括场景模型和触发器等）
    eResFbx,             //fbx资源
    eResEffect,          //战斗特效
    eResFont,            //字体
    eResLua,             //Lua文件
    eResShader,          //Shader文件
    eResMaterial,       //材质球
    eResTexure,         //贴图 主要用于材质球
    eResFontAsset,          //资源配置文件
    eSceneLoadRes,          //场景中动态加载的物体
    eResCount,
}

public class ResDefine
{

    public const string ExtName = ".unity3d";                   //素材扩展名
    public static Dictionary<EResType, string> kDictResPath = new Dictionary<EResType, string>();
    public static string FileList = "filelist.json";
    static void Init()
    {
        kDictResPath[EResType.eResScene] = AppConst.ResourcePath + "/Scene";
        kDictResPath[EResType.eResEffect] = AppConst.ResourcePath + "/Effect";
        kDictResPath[EResType.eResUIEffect] = AppConst.ResourcePath + "/UIEffect";
        kDictResPath[EResType.eResCharacter] = AppConst.ResourcePath + "/Character";
        kDictResPath[EResType.eResBoss] = AppConst.ResourcePath + "/Boss";
        kDictResPath[EResType.eResPet] = AppConst.ResourcePath + "/Pet";
        kDictResPath[EResType.eResMonster] = AppConst.ResourcePath + "/Monster";
        kDictResPath[EResType.eResAudio] = AppConst.ResourcePath + "/Audio";
        kDictResPath[EResType.eResUI] = AppConst.ResourcePath + "/UI";
        kDictResPath[EResType.eResModel] = AppConst.ResourcePath + "/Model";
        kDictResPath[EResType.eResFont] = AppConst.ResourcePath + "/Font";
        kDictResPath[EResType.eResPicture] = Application.dataPath + "/Picture";
        kDictResPath[EResType.eResScene] = Application.dataPath + "/Scenes";
        kDictResPath[EResType.eResShader] = Application.dataPath + "/Shaders";
        kDictResPath[EResType.eResMaterial] = AppConst.ResourcePath + "/Materials";
		kDictResPath[EResType.eResTexure] = AppConst.ResourcePath + "/Texture";
        kDictResPath[EResType.eResFontAsset] = AppConst.ResourcePath + "/FontAssets";
        kDictResPath[EResType.eSceneLoadRes] = AppConst.ResourcePath + "/SceneLoadRes";
    }

    static public string GetResPath(EResType eResourceGroup)
    {
        if(kDictResPath.Count < 1)
           Init();
        if (kDictResPath.ContainsKey(eResourceGroup))
            return kDictResPath[eResourceGroup];
        return null;
    }

    static public string GetResourceType(EResType eResourceGroup)
    {
        switch (eResourceGroup)
        {
            case EResType.eResAudio:
                return "Audio";
            case EResType.eResEffect:
                return "Effect";
            case EResType.eResUI:
                return "UI";
            case EResType.eResCharacter:
                return "Character";
            case EResType.eResModel:
                return "Model";
            case EResType.eResBoss:
                return "Boss";
            case EResType.eResPicture:
                return "Picture";
            case EResType.eResMonster:
                return "Monster";
            case EResType.eResPet:
                return "Pet";
            case EResType.eResUIEffect:
                return "UIEffect";
            case EResType.eResScene:
                return "scenes";
            case EResType.eResFont:
                return "Font";
            case EResType.eResShader:
                return "shader";
            case EResType.eResMaterial:
                return "Materials";
            case EResType.eResTexure:
			    return "Texture";
            case EResType.eResFontAsset:
                return "FontAssets";
            case EResType.eSceneLoadRes:
                return "SceneLoadRes";
        }
        return "";
    }

    public static void GetAllFile(DirectoryInfo kSource, ref List<FileInfo> kfiles,string pattern)
    {
        DirectoryInfo[] DirSource = kSource.GetDirectories();
        foreach (var v in DirSource)
        {
            GetAllFile(v, ref kfiles, pattern);
        }
        FileInfo[] Files = kSource.GetFiles(pattern);
        foreach (var singlefile in Files)
        {
            kfiles.Add(singlefile);
        }
    }

    static string GetFileExt(string path)
    {
        for (int n = path.Length - 1; 0 <= n; n--)
            if (path[n] == '.')
                return path.Substring(n + 1);
        return "";
    }

    static public void GetResTypeFileExtList(EResType eRestype, ref List<string> kExtList)
    {
        kExtList.Clear();
        switch (eRestype)
        {
            case EResType.eResBoss:
            case EResType.eResCharacter:
            case EResType.eResEffect:
            case EResType.eResMonster:
            case EResType.eResPet:
            case EResType.eResUI:
            case EResType.eResUIEffect:
            case EResType.eResModel:
            case EResType.eSceneLoadRes:
                kExtList.Add("prefab");
                break;
            case EResType.eResScene:
                kExtList.Add("unity");
                break;
            case EResType.eResAudio:
                kExtList.Add("mp3");
                kExtList.Add("ogg");
                kExtList.Add("wav");
                break;
            case EResType.eResPicture:
                kExtList.Add("png");
                break;
            case EResType.eResFbx:
                kExtList.Add("FBX");
                break;
            case EResType.eResFont:
                kExtList.Add("ttf");
                break;
            case EResType.eResShader:
                kExtList.Add("shader");
                kExtList.Add("cg");
                kExtList.Add("CG");
			    kExtList.Add("cginc");
                break;
            case EResType.eResMaterial:
                kExtList.Add("mat");
                break;
            case EResType.eResTexure:
                kExtList.Add("png");
                kExtList.Add("tga");
                kExtList.Add("bmp");
                kExtList.Add("jpg");
                kExtList.Add("dds");
                break;
            case EResType.eResFontAsset:
                kExtList.Add("asset");
                kExtList.Add("mat");
                break;
        }
    }


    public static ArrayList GetResourceFiles(string strDir, List<string> kExtList)
    {
        if (strDir[strDir.Length - 1] != '/' && strDir[strDir.Length - 1] != '\\')
            strDir = strDir + "/";
        ArrayList retArray = new ArrayList();

        DirectoryInfo dir = new DirectoryInfo(strDir);
        // CheckDir
        if (dir.Exists == false)
            return retArray;

        FileInfo[] info = dir.GetFiles();

        foreach (FileInfo fileInfo in info)
        {
            if (fileInfo.Name.Contains(".meta"))
                continue;

            string compareName = GetFileExt(fileInfo.Name).ToLower();

            foreach (var v in kExtList)
            {
                if (v == compareName)
                {
                    string retName = strDir + fileInfo.Name;
                    retArray.Add(retName);
                    break;
                }
            }

        }
        return retArray;

    }

    public static void EncryptAssetbundle(string path)
    {
        //Debug.Log("------EncryptAssetbundle------: " + path);
        FileStream kFileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        try
        {
            long len = kFileStream.Length;
            byte[] header = new byte[32];
            kFileStream.Read(header, 0, header.Length);
            string ABMark = System.Text.UTF8Encoding.UTF8.GetString(header, 0, 5);
            if (ABMark.CompareTo(@"Unity") == 0)
            {
                kFileStream.Seek(0, SeekOrigin.Begin);
                string xorkey = @"TLBy_Unity3d";
                for (int i = 0; i < header.Length; i++)
                {
                    header[i] ^= (byte)((byte)xorkey[i % xorkey.Length] ^ (byte)len);
                }
                kFileStream.Write(header, 0, header.Length);
                kFileStream.Flush();
            }
            kFileStream.Close();
        }
        finally
        {
            if (kFileStream != null)
            {
                kFileStream.Dispose();
            }
        }
    }

    public static byte[] DecryptAssetbundle(byte[] kBytes)
    {
        long len = kBytes.Length;

        byte[] dcryptBytes = new byte[len];
        /*
#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            //...自己映射一个函数
            long headerLen = 32;
            string xorkey = @"TLBy_Unity3d";
            for (int i = 0; i < len; i++)
            {
                dcryptBytes[i] = kBytes[i];
            }
            for (int k = 0;k < headerLen; k++)
            {
                dcryptBytes[k] ^= (byte)((byte)xorkey[k % xorkey.Length] ^ (byte)len);
            }
           return dcryptBytes;
        }
        catch (System.Exception ex)
        {
             //Debug.Log("ABMemCreate Error: " + ex.Message);
        }
#else
         * 
 #endif
         * */

        long headerLen = 32;
        string xorkey = @"TLBy_Unity3d";
        for (int i = 0; i < len; i++)
        {
            dcryptBytes[i] = kBytes[i];
        }
        if (len < headerLen)
            headerLen = len;
        for (int k = 0; k < headerLen; k++)
        {
            dcryptBytes[k] ^= (byte)((byte)xorkey[k % xorkey.Length] ^ (byte)len);
        }
        return dcryptBytes;

    }

   
}

public class ResInfo
{
    public string ResName = "";
    public string md5="";
}

public class FilesConfig
{
    public VersionInfo VerRevision;
    public bool AutoUpdate = true;
    public string PatchUrl = string.Empty;
    public List<ResInfo> mShardInfos = new List<ResInfo>();
}

public class VersionInfo
{
    int Major = 1;//1-99
    int Minor = 0;//0-999
    string Revision;//0-99999
    public string version;
    public void Init(int major, int minor, string revision)
    {
        Major = major;
        minor = Minor;
        Revision = revision;
        version = GetVersionTxt();
    }
    public int GetBaseInt()
    {
        return ((((this.Major * 1000 * 1000) + (this.Minor * 1000)) +  int.Parse(this.Revision)));
    }

    public string GetBaseTxt()
    {
        object[] args = new object[] { this.Major, this.Minor };
        return string.Format("{0}.{1}", args);
    }

    public string GetTxt()
    {
        return version;
    }

    string GetVersionTxt()
    {

        object[] args = new object[] { this.Major, this.Minor, this.Revision };
        return string.Format("{0}.{1}.{2}", args);
    }

    public string GetVersion()
    {
        string str = "";
        string[] files = version.Split('.');
        if (files.Length == 3) str = files[2];
        return str;
    }
}

public static class JsonHelp
{   
    static public bool FileExists(string relativePath)
    {
        string path = Application.dataPath + "/" + relativePath; 
        return File.Exists(path);
    }

    static void MakeDirection(string szFloder)
    {
        if (!Directory.Exists(szFloder))
        {
            Directory.CreateDirectory(szFloder);
        }
    }

    public static void MakeDirectionByFileName(string relativePath)
    {
        string path = Application.dataPath + "/" + relativePath;
        MakeDirection(Path.GetDirectoryName(path));
    }

    static public bool WriteAllBytes(string path, byte[] bytes)
    {
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (System.Exception ex)
        {
           UnityEngine.Debug.LogWarning(ex.ToString());
            return false;
        }
        return true;
    }
    static public bool SaveToJsonFile<T>(T tObject, string relativePath)
    {
        MakeDirectionByFileName(relativePath);
        string value = JsonMapper.ToJson(tObject);
        string fileStreamName = Application.dataPath + "/" + relativePath;

        FileStream fileStream = null;
        StreamWriter streamWriter = null;
        try
        {
            fileStream = new FileStream(fileStreamName, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(value);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
        catch (System.Exception ex)
        {
            if (streamWriter != null)
            {
                Debug.LogWarning(ex.ToString());
                streamWriter.Close();
                if (fileStream != null)
                    fileStream.Close();
            }
            return false;
        }
        return true;
    }
    static public T ReadFromJsonString<T>(string value)
    {
        T ret = default(T);
        try
        {
            ret = JsonMapper.ToObject<T>(value);
        }
        catch (System.Exception ex)
        {
            ret = default(T);
            Debug.LogWarning(ex.ToString());
        }

        return ret;
    }

    static public T LoadFromJsonFile<T>(string path)
    {
        StreamReader streamReader = null;
        FileStream fileStream = null;
        T ret = default(T);
        try
        {
            fileStream = new FileStream(path, FileMode.Open);
            streamReader = new StreamReader(fileStream);
            ret = JsonMapper.ToObject<T>(streamReader.ReadToEnd());
            streamReader.Close();
            fileStream.Close();
        }
        catch (System.IO.IOException ex)
        {
            if (streamReader != null)
            {
                string text = streamReader.ReadToEnd();
                if (text == null)
                {
                    Debug.LogWarning("reader.ReadToEnd() = null");
                }
                else
                {
                    Debug.LogWarning("reader.ReadToEnd().Len = " + text.Length);
                }
                Debug.LogWarning(streamReader.ReadToEnd());
                Debug.LogWarning(ex.ToString());
                streamReader.Close();
                if (fileStream != null)
                    fileStream.Close();
            }
            ret = default(T);
        }
        return ret;

    }

}