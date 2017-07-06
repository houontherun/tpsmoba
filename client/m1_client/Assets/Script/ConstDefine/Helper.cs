/********************************************************************
	purpose:	一些函数，无处归类，统一放在这里
*********************************************************************/
#define TestAsync_
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Security;
//using LuaInterface;
using System.Collections;
using System.Security.Cryptography;
using System.Reflection;
public class Helper
{
  
    // 获取字符串的MD5码
    public static string getMD5Str(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString().ToLower();
    }
    // hmacSHA1加密方法
    public static string HmacSha1Sign(string text, string key)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(text);
        byte[] byteKey = Encoding.UTF8.GetBytes(key);
        System.Security.Cryptography.HMACSHA1 hmac = new System.Security.Cryptography.HMACSHA1(byteKey);
        System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(Stream.Null, 
            hmac,
            System.Security.Cryptography.CryptoStreamMode.Write);
        cs.Write(byteData, 0, byteData.Length);
        cs.Close();
        return Convert.ToBase64String(hmac.Hash);
    }

   
    
    // 当前登录游戏的帐号 密码 区ID
    public static string tUserName_ = "zz19";
    public static string tPassWord_ = "1";
    public static uint tZoneID_ = 101;
    public static string tZoneName_;


    // 退出游戏标示 
    public static bool is_exit_confirm_exist_ = false;
    //
    // 订阅回调，玩家登陆成功之后，用于uc和小米渠道
    public delegate void DLoigned();
    public static event DLoigned eDLoigned = null;
    public static void PostEvent_Loigned()
    {
        if (null != eDLoigned)
        {
            eDLoigned();
        }
    }
    // 反外挂，修改器，加密数据
    public static string AntiCheatEncode(int src)
    {
        return Encode(src.ToString());
    }
    public static int AntiCheatDecode(string dst)
    {
        return Convert.ToInt32(Decode(dst));
    }
    // 加入标示符号，以此判断是否修改了内存
    public static string Encode(string data)
    {
        string ret="";
        for (int i = 0; i < data.Length; ++i)
        {
            ret += "0";
            ret += data[i];
        }
        return ret;
    }

    public static string Decode(string data)
    {
        string ret = "";
        for (int i = 0; i < data.Length; i += 2)
        {
            // cheater！！
            if(data[i] != '0')
            {
                ret = "0";
                break;
            }
            ret += data[i+1];
        }
        return ret;
    }
    // 网络消息流水号，防止发包外挂
    public static byte tPackageSerialNumber = 0;


    //--------------- add 20160707 ----------------------
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

    public static System.Type GetType(string classname)
    {
        Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
        System.Type t = null;
        t = assb.GetType(classname); ;
        if (t == null)
        {
            t = assb.GetType(classname);
        }
        return t;
    }
    /// <summary>
    /// 是不是苹果平台
    /// </summary>
    /// <returns></returns>
    public static bool isApplePlatform
    {
        get
        {
            return Application.platform == RuntimePlatform.IPhonePlayer ||
                   Application.platform == RuntimePlatform.OSXEditor ||
                   Application.platform == RuntimePlatform.OSXPlayer;
        }
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
    /// 是否为数字
    /// </summary>
    public static bool IsNumber(string strNumber)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]");
        return !regex.IsMatch(strNumber);
    }

    static public void AddChild(GameObject parent, GameObject prefab)
    {
        if (prefab != null && parent != null)
        {
           Transform t = prefab.transform;
           t.SetParent(parent.transform);
           t.localRotation = Quaternion.identity;
           t.localScale = Vector3.one;
           prefab.layer = parent.layer;
        }
    }
}
