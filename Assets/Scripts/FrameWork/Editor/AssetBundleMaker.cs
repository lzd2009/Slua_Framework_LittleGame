using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
public class AssetBundleMaker  {


    //删除指定目录下所有文件，如果目录不存在，目录将被创建
    private static void ClearTaretDirAllFiles(string dirPath)
    {
        if(Directory.Exists(dirPath))
        {
            string[] filesPath = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < filesPath.Length; i++)
            {
                File.Delete(filesPath[i]);
            }
        }
        else
        {
            Directory.CreateDirectory(dirPath);
        }
        
    }


    [MenuItem("Tool/a.生成当前平台对应的AssetBundle，放在AssetBundles路径下(将删除上次生成的ab包)")]
    public static void MakeAssetBundle()
    {
        ClearTaretDirAllFiles(Tools.AssetBundleCreatePath);
        
        BuildTarget target = BuildTarget.StandaloneWindows64;
#if UNITY_STANDALONE
        target = BuildTarget.StandaloneWindows64;
#elif UNITY_IPHONE
        target = BuildTarget.iOS;
#elif UNITY_ANDROID
        target = BuildTarget.Android;
#endif
        BuildPipeline.BuildAssetBundles(Tools.AssetBundleCreatePath, BuildAssetBundleOptions.None, target);
        AssetDatabase.Refresh();
        Debug.Log("a.制作新的AB包成功，位于:" + Tools.AssetBundleCreatePath);
        string Path = "Assets/" + Tools.AssetBundleCreateDirName + "/" + "scenes.ab";
        string[] files = Directory.GetFiles(Application.dataPath + "/"+  Tools.AssetBundleScenesDirName, "*.unity", SearchOption.AllDirectories);
        string[] relativeFiles = new string[files.Length];
        for (int i = 0;i< files.Length;i++)
        {
            int startIndex = files[i].IndexOf("Assets/");
            relativeFiles[i] = files[i].Substring(startIndex);
        }
        BuildPipeline.BuildPlayer(relativeFiles, Path, target, BuildOptions.BuildAdditionalStreamedScenes);
        AssetDatabase.Refresh();
        Debug.Log("打包场景完成");
    }




    [MenuItem("Tool/b.给最外层的ab包也添加上.ab扩展名，方便用代码管理")]
    public static void AddExtension_ab()
    {
        string sourceFileName = Tools.AssetBundleCreatePath + Tools.AssetBundleCreateDirName;
        string destFileName = sourceFileName + ".ab";
        File.Move(sourceFileName, destFileName);
        Debug.Log("b.添加扩展名成功");
    }

    [MenuItem("Tool/c.生成version.txt文件放入persistentDataPath中存储")]
    public static void CreateVersionTextFile()
    {
        // 生成记录所有MD5的version.txt文件到persistentDataPath文件夹
        FileStream stream = new FileStream(Application.persistentDataPath + "\\version.txt", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(Tools.GetAllMD5HashFromAllAssetBundle());
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
        Debug.Log("c.生成version文件成功");
    }

    [MenuItem("Tool/d.汇总更新文件到UpdatePacage文件夹，把它上传到服务器根目录吧")]
    public static void CreateUpdatePackage()
    {
        ClearTaretDirAllFiles(Tools.FinalUpdatePackagePath);
        string versiontxtPath = Application.persistentDataPath + "/version.txt";
        string[] files = Directory.GetFiles(Tools.AssetBundleCreatePath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string extension = filePath.Substring(files[i].LastIndexOf("."));
            if (extension == ".ab")
            {
                string[] s = filePath.Split('/');
                string localPath = s[s.Length - 1];
                File.Copy(filePath, Tools.FinalUpdatePackagePath + "/"+localPath);
            }
        }
        File.Copy(versiontxtPath,Tools.FinalUpdatePackagePath + "version.txt");
        Debug.Log("d.汇总更新文件成功，请将UpdatePackage文件夹内所有文件拖放到服务器");
    }

    [MenuItem("Tool/e.模拟旧版本客户端。（将删除persistentDataPath中的所有文件，包括ab包文件和version.txt文件）")]
    public static void ClearCache()
    {
        Tools.DeleteDir(Application.persistentDataPath);
        Debug.Log("e.模拟成未更新版本成功");
    }



}
