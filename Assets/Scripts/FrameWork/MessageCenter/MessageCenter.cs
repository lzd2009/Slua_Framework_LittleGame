using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SLua;


[CustomLuaClass]
public delegate void EventHandler(LuaTable self,string eventName,object arg);
[CustomLuaClass]
public class MessageCenter  {

    private static Dictionary<string, List<LuaTable>> handlerDic = new Dictionary<string, List<LuaTable>>();

    public static void AddListener(string eventName, LuaTable self)
    {
        List<LuaTable> attentionList = null;
        handlerDic.TryGetValue(eventName,out attentionList);
        if (attentionList == null)
        {
            attentionList = new List<LuaTable>();
            handlerDic.Add(eventName, attentionList);
        }
        attentionList.Add(self);
    }
    public static void RemoveListener(string eventName,LuaTable self)
    {
        List<LuaTable> attentionList = null;
        handlerDic.TryGetValue(eventName, out attentionList);
        if(attentionList == null)
        {
            return;
        }
        attentionList.Remove(self);
    }


    public static void SendEvent(string eventName,object arg = null)
    {
        List<LuaTable> attentionList = null;
        handlerDic.TryGetValue(eventName, out attentionList);
        if(attentionList != null && attentionList.Count != 0)
        {
            for (int i = 0; i < attentionList.Count; i++)
            {
                LuaFunction luaFunc = null;
                try
                {
                    luaFunc = (LuaFunction)((attentionList[i])["eventHandler"]);
                }
                catch (Exception e)
                {
                    Debug.Log("请检查监听了事件：" + eventName + "的table中是否存在对应的处理函数：eventHandler   " + e);
                }
                EventHandler handler = luaFunc.cast<EventHandler>();
                handler(attentionList[i], eventName, arg);
            }
        }
    }

    //当切换关卡时，自动调用这个函数
    public static void ClearAllEvent()
    {
        handlerDic.Clear();
    }

    
}
