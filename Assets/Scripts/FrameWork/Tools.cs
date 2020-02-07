using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
[CustomLuaClass]
public class Tools {

    #region 文件夹设定
    //ab包生成的文件夹
    public static string AssetBundleCreateDirName = "AssetBundles";
    public static string AssetBundleCreatePath {
        get { return Application.dataPath + "/" + AssetBundleCreateDirName + "/"; }
    }
    //要打到ab包里的场景放在这个文件夹中
    public static string AssetBundleScenesDirName = "Scenes/AssetBundleScenes";

    //所有更新文件的汇总文件夹
    public static string FinalUpdatePackageDirName = "UpdatePackage";
    public static string FinalUpdatePackagePath {
        get { return Application.dataPath + "/" + FinalUpdatePackageDirName + "/"; }
    }

    public static string SceneAssetBundleDirName = "ScenesAssetBundles";


    #endregion

    //复制文件
    public static void CopyFile(string sourceFile, string toFile)
    {
        File.Copy(sourceFile, toFile, true);
    }

    //删除文件夹
    public static void DeleteDir(string DirPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(DirPath);
        directoryInfo.Delete(true);
    }
    //删除文件
    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    //生成文件的md5值
    public static string GetMD5HashFromFile(string filePath)
    {
        try
        {
            FileStream file = new FileStream(filePath, FileMode.Open);//获取文件对象
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retval = md5.ComputeHash(file);//生成MD5值
            file.Close();
            StringBuilder sb = new StringBuilder();

            foreach (var r in retval)
            {
                sb.Append(r.ToString("x4"));
            }
            return sb.ToString();
        }
        catch (Exception e)
        {
            throw new Exception("GetMD5Hash Failed!" + e.ToString());
        }
    }

    //拿到包含所有ab包的相对路径，md5值的 字符串。
    public static string GetAllMD5HashFromAllAssetBundle()
    {
        string versiontxtPath = Application.persistentDataPath;//版本文件的目录
        string[] files = Directory.GetFiles(AssetBundleCreatePath, "*", SearchOption.AllDirectories);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            string extension = filePath.Substring(files[i].LastIndexOf("."));
            if (extension == ".ab")
            {
                string relativePath = filePath.Replace(AssetBundleCreatePath, "").Replace("\\", "/");
                string md5 = GetMD5HashFromFile(filePath);//生成文件的MD5值
                sb.Append(relativePath).Append(",").Append(md5).Append("\n");
            }
        }
        return sb.ToString();
    }

}
