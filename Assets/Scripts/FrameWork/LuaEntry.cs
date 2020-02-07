using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SLua;
using System;
[CustomLuaClass]
public class LuaEntry : LuaBehaviour {

    private LuaSvr luaSvr;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SetLuaSvr();
        LuaTable mainTable = AssetManager.Require("main.txt");
        BindLuaTable(mainTable);
    }

    #region 设置lua环境
    private void SetLuaSvr()
    {
        luaSvr = new LuaSvr();
        //init方法的调用是必要的，不然lua端import UnityEngine将报错。
        luaSvr.init(null, ()=> { LuaSvr.mainState.loaderDelegate += MyLuaFileLoader; });      
    }


    private byte[] MyLuaFileLoader(string fn,ref string s)
    {
        string[] str = fn.Split('.');
        if (str.Length != 2)
            Debug.Log("lua文件后缀不标准，请使用txt后缀例如：lua1.txt");
        string fileName_withoutExtend = str[0];
        return  AssetManager.GetLuaFileData(fileName_withoutExtend);
    }


    #endregion

}
