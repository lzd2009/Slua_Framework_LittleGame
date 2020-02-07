using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;
[CustomLuaClass]
public class UIManager {

    private static Transform Canvas;
    //管理所有当前在内存中的panel物体，键为panel预制体名字，值为panelGo
    private static Dictionary<string, GameObject> panelDic;
    static UIManager()
    {
        panelDic = new Dictionary<string, GameObject>();
        GameObject canvasPrefab = (GameObject)AssetManager.LoadAsset("canvas.ab", "Canvas");
        GameObject canvasGo = GameObject.Instantiate<GameObject>(canvasPrefab);
        UnityEngine.Object.DontDestroyOnLoad(canvasGo);
        Canvas = canvasGo.transform;
    }
    //显示一个panel，由字典panelDic管理
    public static GameObject ShowPanel(string abName,string panelName)
    {
        GameObject panelGo = null;
        bool exist = panelDic.TryGetValue(panelName, out panelGo);
        if(!exist)
        {
            GameObject panelPrefab = (GameObject)AssetManager.LoadAsset(abName, panelName);
            panelGo = GameObject.Instantiate(panelPrefab);
            panelGo.transform.SetParent(Canvas, false);
            panelDic.Add(panelName, panelGo);
        }
        return panelGo;
    }
    //关卡跳转的时候调用。因为canvas设置了跳场景不销毁，所以要手动清除ui。
    public static void ClearAllPanel()
    {
        if (panelDic.Count == 0)
            return;
        foreach (var item in panelDic)
        {
            GameObject.Destroy(item.Value);
        }
        panelDic.Clear();
    }

}
