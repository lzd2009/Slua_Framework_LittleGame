使用slua的一个简单的框架。
框架部分都在C#端编写，包括：
资源热更新：计算每个assetBundle的md5值->记录到一个version.txt文件中->将assetBundle们和version.txt上传到服务器。客户端启动时，首先看本地有无version.txt文件并且与服务器version.txt文件进行比对，把本地没有的和md5值不同的ab包的名字们记录下来，然后从服务器下载到本地。
lua文件的加载：lua文件全部打包到同一个assetBundle，叫做lua.ab。在资源热更完成后，从lua.ab中取出所有lua文件，用文件名，byte[]存储到字典里。doFile的时候从字典里取。
lua与unity生命周期函数的交互：写了一个LuaBehaviour.cs脚本继承monoBehaviour，一个LuaBehaviour.cs绑定一个lua table。将start,update,fixedUpdate等方法套接到lua中执行。在lua端使用的时候，代码应该像这样：
    local characterAsset = AssetManager.LoadAsset('character.ab','character');
    local characterGo = GameObject.Instantiate(characterAsset);
    local Character = AssetManager.Require('character.txt') 
    local character = Character:new(); 
    characterGo:AddComponent(LuaBehaviour):BindLuaTable(character);
资源管理器assetManager：想要从指定ab包中取出指定名字的资源，传入ab包名，资源名，资源管理器将加载指定ab包（若有依赖，依赖包也将加载）并取出指定资源，取出后，将直接assetBundle.UnLoad(false)把加载的ab包和依赖包都卸载掉。回收一部分内存。加载出来的asset将由assetManager添加到字典里管理，键为资源名，值为asset本身。下次再想加载这个资源将从字典里直接取出返回。切换关卡的时候需要清空这个字典。让Resources.UnLoadUnusedAssets()方法能够回收这些资源。
对象池：写了一个gameObject的对象池。
消息派发：使用监听者模式。消息中心维护了一个字典：Dictionary<string, List<LuaTable>> handlerDic。string就是事件名，list就是关心这个事件的所有table。没有使用c#的委托来实现监听者模式，因为那样做的话，在事件触发时，lua端的方法，拿不到self变量。
有限状态机：在lua端写了角色和敌人的有限状态机。


