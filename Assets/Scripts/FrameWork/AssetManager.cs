using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SLua;
using UnityEngine.SceneManagement;
//这个类专门管理运行时的AB包，管理从AB包load出来的asset
[CustomLuaClass]
public class AssetManager {

    #region 对lua文件的管理
    //这个字典在游戏开始时被初始化，加载所有lua代码文件到内存中备用。
    private static Dictionary<string, byte[]> luaFileDic = new Dictionary<string, byte[]>();
    public static void LoadAllLuaFileToMemory()
    {
        AssetBundle luaTextAssetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/lua.ab");
        string[] assetNames = luaTextAssetBundle.GetAllAssetNames();           
        TextAsset[] allLuaFileAsset = luaTextAssetBundle.LoadAllAssets<TextAsset>();
        
        for (int i = 0; i < allLuaFileAsset.Length; i++)
        {
            string[] str = assetNames[i].Split('/');
            string[] s = (str[str.Length - 1]).Split('.');
            string key = s[0];
            luaFileDic.Add(key, allLuaFileAsset[i].bytes);
        }
    }
    /// <summary>
    /// 框架内部方法，请勿调用
    /// </summary>
    /// <param name="luaFileName">别加.txt后缀</param>
    /// <returns></returns>
    public static byte[] GetLuaFileData(string luaFileName)
    {
        if(luaFileDic.ContainsKey(luaFileName) == false)
        {
            Debug.LogError("无法加载指定lua文件：" + luaFileName + "请： 1.检查名字是否匹配。2.检查是否给txt文件打上了lua.ab的标签。");
        }
        return luaFileDic[luaFileName];
    }


    private static Dictionary<string, LuaTable> loadedLuaTableDic = new Dictionary<string, LuaTable>();

    /// <summary>
    /// 对外静态方法，通过一个字典，在c#端模拟了lua端的require函数的实现，将返回一张表。
    /// </summary>
    /// <param name="luaFileName">lua文件名，加.txt后缀</param>
    /// <returns></returns>
    public static LuaTable Require(string luaFileName)
    {
        LuaTable luaTable = null;
        bool existed = loadedLuaTableDic.TryGetValue(luaFileName, out luaTable);
        if (existed)
        {
            return luaTable;
        }
        else
        {
            luaTable = (LuaTable)LuaSvr.mainState.doFile(luaFileName);
            if (luaTable == null)
                Debug.Log("lua文件：" + luaFileName + "未返回一个table，请检查");
            loadedLuaTableDic.Add(luaFileName, luaTable);
            return luaTable;
        }
    }

    #endregion


    public static void InitAssetManager()
    {
        LoadAllLuaFileToMemory();
        InitManifest();
    }
    private static void InitManifest()
    {
        //原来的
        //AssetBundle bundle = AssetBundle.LoadFromFile(Application.persistentDataPath +"/"+ Tools.AssetBundleCreateDirName + ".ab");
        //assetManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //2020.4.9修改：
        AssetBundle bundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + "StreamingAssets.ab");
        assetManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

    }

    #region 对其他资源文件的管理
    //这个字典用来管理从ab包里加载出来的asset key的格式例如：character.ab(包名)#character1(asset名)
    private static Dictionary<string, UnityEngine.Object> assetDic = new Dictionary<string, UnityEngine.Object>();
    private static AssetBundleManifest assetManifest;
    private static void AddAssetToDic(string key, UnityEngine.Object asset)
    {
        if (assetDic.ContainsKey(key) == false)
        {
            assetDic.Add(key, asset);
        }
        else
        {
            //中了依赖包加载相关的逻辑
            Debug.Log("字典里已经有这个资源了，请检查逻辑");
        }
    }
    private static UnityEngine.Object TryToGetAsset(string key)
    {
        if (assetDic.ContainsKey(key) == true)
        {
            return assetDic[key];
        }
        else
        {
            return null;
        }
    }
    private static List<AssetBundle> LoadDependenceBundles(string bundleName)
    {
        string[] dependencePaths = assetManifest.GetAllDependencies(bundleName);
        if(dependencePaths.Length == 0)
            return null;
        List<AssetBundle> dependenceBundles = new List<AssetBundle>();
        for (int i = 0;i<dependencePaths.Length;i++)
        {
            AssetBundle ab = AssetBundle.LoadFromFile(Application.persistentDataPath + "/"+ dependencePaths[i]);
            dependenceBundles.Add(ab);
        }
        return dependenceBundles;
    }

    /// <summary>
    /// 加载本地ab包内的资源。加载ab包内资源请只使用这个函数
    /// </summary>
    /// <param name="localABName">ab包相对路径url即可，不需要加persistentDataPath，要加.ab后缀</param>
    /// <param name="assetName">ab包内的资源名称，不需要后缀</param>
    /// <returns></returns>
    public static UnityEngine.Object LoadAsset(string relativeABName, string assetName)
    {
        string key = relativeABName + "#" + assetName;
        UnityEngine.Object asset = TryToGetAsset(key);
        if (asset != null)
        {
            return asset;
        }
        else
        {
            //没有，需要从ab包加载这个asset，并加入字典 
            //首先加载依赖包
            string abName = relativeABName;
            if (relativeABName.Contains("/"))
            {
                //包在文件夹内，需要去掉相对路径，只留包名
                string[] s = relativeABName.Split('/');
                abName = s[s.Length - 1];
                Debug.Log("包在某个文件夹中");
            }
            List<AssetBundle> dependenceBundles = LoadDependenceBundles(abName);
            AssetBundle bundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + relativeABName);           
            if (bundle != null)
            {
                asset = bundle.LoadAsset(assetName);
                //添加进字典                
                AddAssetToDic(key, asset);
                //直接卸载ab包，和依赖的ab包
                bundle.Unload(false);
                if (dependenceBundles != null)
                {
                    for(int i = 0;i<dependenceBundles.Count;i++)
                    {
                        dependenceBundles[i].Unload(false);
                    }
                }
                return asset;
            }
            else
            {
                Debug.Log("加载指定ab包失败，请检查参数");
                return null;
            }
        }
    }


    /// <summary>
    /// 如果要卸载由ab包产生的asset，请调用这个方法。例如卸载prefab,texture
    /// </summary>
    /// <param name="relativeABName">ab包的名字，例如：characters.ab</param>
    /// <param name="assetName">asset名字，例如character1</param>
    public static void UnloadAsset(string relativeABName,string assetName)
    {
        string key = relativeABName + "#" + assetName;
        UnityEngine.Object asset = TryToGetAsset(key);
        if(asset == null)
        {
            Debug.LogError("请检查参数 key:"+key);
        }
        else
        {
            assetDic.Remove(key);
            Resources.UnloadAsset(asset);           
        }
    }

    public static void ClearAllAsset()
    {
        assetDic.Clear();
        Resources.UnloadUnusedAssets();
    }

    #endregion

    
}
