using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
public class UpdateVersion : MonoBehaviour {

    //记录了本地所有ab文件的(文件名,md5)的txt文件名字
    private string localVersionTextName = "version.txt";
    //远程服务器的资源下载地址
    private const string server_res_url = @"http://39.97.232.127/";
    //本地ab包们的 <文件名,md5>字典
    private Dictionary<string, string> localVersion = new Dictionary<string, string>();
    //服务器ab包们的 <文件名,md5>字典
    private Dictionary<string, string> serverVersion = new Dictionary<string, string>();
    //对比本地ab包们的md5和服务器ab包们的md5，得出来的变化了的需要更新的那些ab包的文件名
    private List<string> needUpdateList = new List<string>();

    //当needUpdateList长度不为0，这个标志位会打上。
    private bool needUpdate = false;

    //当下载出现错误，这个标志位会打上，错误的时候将不会更新本地版本文件，下次再下载。
    private bool downLoadHasError = false;

    private void DevelopMode()
    {
        //将AssetBundle下的文件拷贝到persistent文件夹。
        string[] files = Directory.GetFiles(Tools.AssetBundleCreatePath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string extension = filePath.Substring(files[i].LastIndexOf("."));
            if (extension == ".ab")
            {
                string[] s = filePath.Split('/');
                string localPath = s[s.Length - 1];
                File.Copy(filePath, Application.persistentDataPath + "/" + localPath);
            }
        }
        //进入逻辑。
        AssetManager.InitAssetManager();
        print("assetManager初始化完成");
        print("开始进入lua逻辑");
        EnterLuaLogic();
    }

    //2020.4.9：完全不需要联机下载资源，放入streamingAsset文件夹，运行时直接将ab包拷贝到present文件夹。
    private void DirectMode()
    {
        //先清除persistent文件夹
        Tools.DeleteDir(Application.persistentDataPath);
        //将StreamingAsset下的文件拷贝到persistent文件夹。
        string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/", "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string extension = filePath.Substring(files[i].LastIndexOf("."));
            if (extension == ".ab")
            {
                string[] s = filePath.Split('/');
                string localPath = s[s.Length - 1];
                File.Copy(filePath, Application.persistentDataPath + "/" + localPath);
            }
        }
        //进入逻辑。
        AssetManager.InitAssetManager();
        print("assetManager初始化完成");
        print("开始进入lua逻辑");
        EnterLuaLogic();
    }

    private void Awake()
    {
        DirectMode();
//#if DEVELOP_MODE
        //Debug.Log("开发模式，读取本地ab包，不需要连接服务器下载。");
        //DevelopMode();
        //return;
//#endif
        //InitLocalVersionDic();
    }

    //读取persistentDataPath中的本地version.txt文件并用于初始化本地版本字典，完成后调用InitServerVersionDic
    private void InitLocalVersionDic()
    {
        string localVersionPath = Application.persistentDataPath + "/" + localVersionTextName;

        if (File.Exists(localVersionPath) == true)
        {
            StartCoroutine(DownLoadFile_WWWway(localVersionPath, (www) => {
                AddVersionTextToVersionDic(www.text, localVersion);
                InitServerVersionDic();
            }));
        }
        else
        {
            InitServerVersionDic();
        }
    }
    //读取服务器的version.txt文件并用于初始化本地版本字典,完成后调用InitNeedUpdateList
    private void InitServerVersionDic()
    {
        string serverVersionPath = server_res_url + "version.txt";
        StartCoroutine(DownLoadFile_WWWway(serverVersionPath, (www) => {
            AddVersionTextToVersionDic(www.text, serverVersion);
                InitNeedUpdateList();
        }));
    }

    //在两个版本字典都加载完成时会被调用，将对比本地版本字典和服务器版本字典，将服务器改变的和新增的ab包的名字，加入待更新列表
    private void InitNeedUpdateList()
    {    
        foreach (var v in serverVersion)//遍历服务器下载的MD5值列表
        {
            if (!localVersion.ContainsKey(v.Key))
            {
                needUpdateList.Add(v.Key);
            }
            else
            {
                string localMD5;//键对应的本地MD5值
                localVersion.TryGetValue(v.Key, out localMD5);
                if (!localMD5.Equals(v.Value))//比较同键的两个MD5值是否相同
                {
                    needUpdateList.Add(v.Key);
                }
            }
        }
        needUpdate = needUpdateList.Count > 0;//当需要更新列表有数据加开启本地版本文件更新
        DownLoadNeedUpdateList();
        
    }

    
    //这里递归下载，一个下载完成再下载下一个，感觉有点慢，以后考虑做ab包文件压缩，或者开多线程下载或者开多coroutine下载，两个方面去优化。
    private void DownLoadNeedUpdateList()
    {
        
        if (needUpdateList.Count == 0)
        {
            //中这条逻辑，就是需要下载，并且下载全部完成了。所以更新版本文件。
            if (needUpdate == true)
            {
                //下载时有错误就返回吧。
                if(downLoadHasError == true)
                {
                    Debug.LogError("下载出错");
                    return;
                }
                print("ab包全部下载完成，开始更新版本文件txt");
                UpdateLocalVersionText();
            }
            else
            {
                print("当前为最新版本，无更新内容");
            }
            AssetManager.InitAssetManager();
            print("assetManager初始化完成");
            print("开始进入lua逻辑");
            EnterLuaLogic();
            return;
        }
        
        string filePath = needUpdateList[0];
        needUpdateList.RemoveAt(0);
        StartCoroutine(this.DownLoadFile_WWWway(server_res_url + filePath, delegate (WWW www)
        {
            if(www.error != null)
            {
                downLoadHasError = true;
                Debug.Log("www has error!:" + www.error);
                OnDownLoadError();
                return;
            }
                
            ReplaceLocalResources(filePath, www.bytes);
            DownLoadNeedUpdateList();
        }));
    }

    private void OnDownLoadError()
    {
        Debug.Log("下载中断...网络出现错误...");
        //todo 错误处理，询问玩家
    }

    /// <summary>
    /// 以覆盖模式将ab包写入本地
    /// </summary>
    /// <param name="filename">传入新增资源名字，相对路径即可</param>
    /// <param name="data">传入新增资源的数据，byte[]格式传入</param>
    private void ReplaceLocalResources(string filename, byte[] data)
    {
        string filePath = Application.persistentDataPath + "/"+ filename;
        FileStream file = new FileStream(filePath, FileMode.Create);
        file.Write(data, 0, data.Length);
        file.Flush();
        file.Close();
    }

    /// <summary>
    /// 根据服务器版本字典，写入并且覆盖本地的version.txt文件
    /// </summary>
    private void UpdateLocalVersionText()
    {
        if (needUpdate)
        {
            StringBuilder version = new StringBuilder();
            foreach (var v in serverVersion)
            {
                version.Append(v.Key).Append(",").Append(v.Value).Append("\n");
            }
         
            FileStream stream = new FileStream(Application.persistentDataPath + "/"+ localVersionTextName, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(version.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }
    }

    /// <summary>
    /// 将文件名,md5/n...的string，加入版本字典
    /// </summary>
    /// <param name="content">从version.text中读取出来的string</param>
    /// <param name="targetVersionDic">版本信息字典</param>
    private void AddVersionTextToVersionDic(string content,Dictionary<string,string> targetVersionDic)
    {
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split('\n');
        foreach (string s in items)
        {
            string[] info = s.Split(',');
            if (info != null && info.Length == 2)
            {
                targetVersionDic.Add(info[0], info[1]);
            }
        }
    }
    
   
    //使用www类来下载web资源
    IEnumerator DownLoadFile_WWWway(string url, Action<WWW> onDownloadFinishHandler)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if(www.isDone== true)
            {
                onDownloadFinishHandler(www);
            }         
        }
    }

    //在全部资源热更新完毕后，会进入lua的逻辑
    private void EnterLuaLogic()
    {
        GameObject luaEntryGo = new GameObject("luaEntry");
        luaEntryGo.AddComponent<LuaEntry>();
    }


}
