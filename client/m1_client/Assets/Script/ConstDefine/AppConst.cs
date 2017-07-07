using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class AppConst
{
	    public const bool PublishMode = false;                    //发布模式
	    public const bool UpdateMode = false;
        public const bool logDebug = true;
        /// <summary>
        /// 如果开启更新模式，前提必须启动框架自带服务器端。
        /// 否则就需要自己将StreamingAssets里面的所有内容
        /// 复制到自己的Webserver上面，并修改下面的WebUrl。
        /// </summary>
        public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 

        public const float TimerInterval = 0.1f;                    //TimerManager每隔0.1s Tick一次(默认情况下)
        public const int GameFrameRate = 30;                        //游戏帧频

        public const string AppName = "TLBY";               //应用程序名称
        public const string LuaTempDir = "Lua/";                    //临时目录
        public const string AppPrefix = AppName + "_";              //应用程序前缀
        public const string AssetDir = "StreamingAssets";           //素材目录 
        public const string PathWebUrl = "http://112.13.170.137/patch/";      //测试更新地址
        public const string PatchListUrl = "http://112.13.170.137/patch_list/";      //测试更新地址
        public const string PublishServerListUrl = "http://tlby-epgs1.ihfgame.com/server_list/apple.json"; //发布的 serverList
        public const string DevServerListUrl = "http://112.13.170.137/server_list/develop.json"; //用于开发 serverList
        public const string PatchList = "patch_list.json";//资源列表文件
        public const string DevServerList = "develop.json";//资源列表文件
        public const string PublishServerList = "apple.json";//资源列表文件
        public static string UserId = string.Empty;        //用户ID
        public static int SocketPort = 0;                           //Socket服务器端口
        public static string SocketAddress = string.Empty;          //Socket服务器地址

        public const string VER_REVISION = "$WCREV$";            //从svn获取版本号
        public const int VER_MAJOR = 1;
        public const int VER_MINOR = 0;

        public static string Version = string.Empty;

        public static string FrameworkRoot
        {
            get
           {
                return Application.dataPath + "/" + AppName;
            }
        }

       public static string ResourcePath
       {
          get
          {
              return Application.dataPath + "/Resources";
          }
       }
    }
