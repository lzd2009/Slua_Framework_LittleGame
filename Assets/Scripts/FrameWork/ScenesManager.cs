using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using SLua;
[CustomLuaClass]
public class ScenesManager {


    private static LuaEntry luaEntry;
    //静态构造函数
    static ScenesManager()
    {        
        luaEntry = GameObject.Find("luaEntry").GetComponent<LuaEntry>();
    }

    public static void LoadAssetBundleScene(string relativeSceneABName, string sceneName, Action<float> loadingHandler, Action<string> loadedHandler)
    {
        //改成 Coroutine 类型，而不是委托，不然报错。不然就lua那边。写成函数。返回ienumber的。先试试lua那边。
        luaEntry.StartCoroutine(LoadAssetBundleScene_Async(relativeSceneABName, sceneName, loadingHandler, loadedHandler));
    }

    private static IEnumerator LoadAssetBundleScene_Async(string relativeSceneABName, string sceneName, Action<float> loadingHandler, Action<string> loadedHandler)
    {
        //我怀疑static类型的coroutine，只能跑一次。再跑一次会出错。todo：test
        string path = Application.persistentDataPath + "/" + relativeSceneABName;
        AssetBundleCreateRequest abRequest = AssetBundle.LoadFromFileAsync(path);
        yield return abRequest;
        AssetBundle bundle = abRequest.assetBundle;
        if (bundle == null)
        {
            Debug.Log("Failed to load AssetBundle!bundle name is :"+relativeSceneABName);
            yield break;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            loadingHandler(operation.progress);
            yield return null;
        }
        //资源管理器清除对上个场景资源的引用。
        AssetManager.ClearAllAsset();
        //消息中心保存的上个场景的引用全部清空。
        MessageCenter.ClearAllEvent();
        loadedHandler(sceneName);
         
        //加载完成了，卸载ab包吧
        bundle.Unload(false);
    }
}
