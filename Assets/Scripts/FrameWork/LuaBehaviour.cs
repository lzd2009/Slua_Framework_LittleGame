using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;
using UnityEngine.SceneManagement;
using System;
[CustomLuaClass]
public class LuaBehaviour : MonoBehaviour {
    public LuaTable table;

    public bool useOnEnable = false;
    public bool useUpdate = false;
    public bool useLateUpdate = false;
    public bool useFixedUpdate = false;
    public bool useOnDisable = false;
    public bool useOnDestroy = false;
    public bool useOnTriggerEnter = false;
    public bool useOnCollisionEnter = false;
    

    protected void Start()
    {
        if (table != null)
            CallLuaFunction("start",table);
    }
    protected void OnEnable()
    {
        if ((table != null) && useOnEnable )
            CallLuaFunction("onEnable",table);
    }
    protected void OnDisable()
    {
        if ((table != null) && useOnDisable)
            CallLuaFunction("onDisable",table);
    }
    protected void Update()
    {
        if ((table != null) && useUpdate)
            CallLuaFunction("update",table);
    }
    protected void FixedUpdate()
    {
        if ((table != null) && useFixedUpdate)
            CallLuaFunction("fixedUpdate",table);
    }

    protected void LateUpdate()
    {
        if ((table != null) && useLateUpdate)
            CallLuaFunction("lateUpdate",table);
    }
    protected void OnDestroy()
    {
        if (table != null && useOnDestroy)
            CallLuaFunction("onDestroy",table);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (table != null && useOnTriggerEnter)
            CallLuaFunction("onTriggerEnter", table, new object[] { other });
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if(table != null && useOnCollisionEnter)
            CallLuaFunction("onCollisionEnter", table, new object[] { collision });
    }

    //绑定new出来的table
    public void BindLuaTable(LuaTable luaTable)
    {
        table = luaTable;
        table["gameObject"] = gameObject;
        table["this"] = this;
        table["transform"] = transform;
    }

    protected void CallLuaFunction(string funcName,LuaTable self, object[] args = null)
    { 
        LuaFunction luaFunc = (LuaFunction)table[funcName];      
        luaFunc.call(self,args);
    }

}
