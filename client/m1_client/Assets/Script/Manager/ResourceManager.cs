using UnityEngine;
using System.Collections;
using System;

public class ResourceManager : Manager
{
    public static GameObject defaultCanvas = GameObject.Find("Canvas");
    public static void CreateCharacter(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResCharacter, timeToRecycle, timeToDestroy,(Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreateCharacter(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreateCharacter(res, timeToRecycle, 120, func);
    }

    public static void CreateCharacter(string res, Action<UnityEngine.Object> func = null)
    {
        CreateCharacter(res, 0, func);
    }

    /// <summary>
    /// 创建场景元素
    /// </summary>
    public static void CreateSceneElemt(string res, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResSceneElemt, 0, 10, (Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreateEffect(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResEffect, timeToRecycle,timeToDestroy,(Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreateEffect(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreateEffect(res, timeToRecycle, 80,func);
    }

    public static void CreateEffect(string res, Action<UnityEngine.Object> func = null)
    {
        CreateEffect(res, 0, func);
    }

    public static void CreateUIEffect(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResUIEffect, timeToRecycle,timeToDestroy, (Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreateUIEffect(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreateUIEffect(res, timeToRecycle,30,func);
    }

    public static void CreateUIEffect(string res, Action<UnityEngine.Object> func = null)
    {
        CreateUIEffect(res, 0, func);
    }

    public static void CreateUI(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {

        ObjectPoolManager.NewObject(res, EResType.eResUI, timeToRecycle, timeToDestroy,(Obj) =>
        {
            if (func != null)
            {
                GameObject go = Obj as GameObject;
                go.transform.SetParent(defaultCanvas.transform.Find("scene"));
                go.transform.localScale = Vector3.one;
                go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                func(go);
            }
        });
    }

    public static void CreateUI(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreateUI(res, timeToRecycle,100, func);
    }

    public static void CreateUI(string res, Action<UnityEngine.Object> func = null)
    {
        CreateUI(res, 0, func);
    }

    public static void CreateModel(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResModel, timeToRecycle,timeToDestroy,(Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreateModel(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreateModel(res, timeToRecycle,100, func);
    }

    public static void CreateModel(string res, Action<UnityEngine.Object> func = null)
    {
        CreateModel(res, 0, func);
    }

    public static void CreatePet(string res, float timeToRecycle, float timeToDestroy, Action<UnityEngine.Object> func = null)
    {
        ObjectPoolManager.NewObject(res, EResType.eResPet, timeToRecycle,timeToDestroy, (Obj) =>
        {
            GameObject gObj = Obj as GameObject;
            if (func != null)
            {
                func(gObj);
            }
        });
    }

    public static void CreatePet(string res, float timeToRecycle, Action<UnityEngine.Object> func = null)
    {
        CreatePet(res, timeToRecycle,100,func);
    }

    public static void CreatePet(string res, Action<UnityEngine.Object> func = null)
    {
        CreatePet(res, 0,func);
    }

    public static Material GetMaterial(string res)
    {
        Material mat = null;
        if (AppConst.PublishMode)
        {
            mat = ObjectPoolManager.GetSharedResource(res, EResType.eResMaterial) as Material;
        }
        else
        {
#if UNITY_EDITOR
            mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/" + res + ".mat");
#endif
        }
        return mat;
        //    return ObjectPoolManager.GetSharedResource(res, EResType.eResMaterial) as Material;
    }

    public static string GetText(string res)
    {
        TextAsset textAsset = null;
        string text = string.Empty;
        if (AppConst.PublishMode)
        {
            textAsset = ObjectPoolManager.GetSharedResource(res, EResType.eResDataTable,false) as TextAsset;
        }
        else
        {
#if UNITY_EDITOR
            textAsset = Resources.Load<TextAsset>("DataTable/" + res);
#endif
        }
        if (textAsset!=null)
        {
            text = textAsset.text;
        }

        return text;
        //    return ObjectPoolManager.GetSharedResource(res, EResType.eResMaterial) as Material;
    }

    public static bool RecycleObject(GameObject go)
    {
        return ObjectPoolManager.RecycleObject(go);
    }

    public static Sprite LoadSprite(string Spritepath)
    {
        Sprite sprite = null;
        if (AppConst.PublishMode)
        {
            sprite = ObjectPoolManager.GetSharedResource(Spritepath, EResType.eResPicture) as Sprite;
        }
        else
        {
#if UNITY_EDITOR
            sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Picture/" + Spritepath + ".png");
#endif
        }
        return sprite;
    }

    public static AudioClip LoadAudioClip(string clipName)
    {
        return ObjectPoolManager.GetSharedResource(clipName, EResType.eResAudio) as AudioClip;
    }

    public static Texture LoadTexture(string Texturepath)
    {
        Texture tex = null;
        if (AppConst.PublishMode)
        {
            tex = ObjectPoolManager.GetSharedResource(Texturepath, EResType.eResTexure) as Texture;
        }
        else
        {
#if UNITY_EDITOR
            tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Texture/" + Texturepath + ".png");
            if (!tex)
                tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Texture/" + Texturepath + ".bmp");
            if (!tex)
                tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Texture/" + Texturepath + ".dds");
            if (!tex)
                tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Texture/" + Texturepath + ".jpg");
            if (!tex)
                tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Texture/" + Texturepath + ".tga");
#endif
        }
        return tex;
    }


}
