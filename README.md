使用slua的一个简单的框架。  
框架部分都在C#端编写，包括：  

资源热更新：计算每个assetBundle的md5值->记录到一个version.txt文件中->将assetBundle们和version.txt上传到服务器。客户端启动时，首先看本地有无version.txt文件并且与服务器version.txt文件进行比对，把本地没有的和md5值不同的ab包的名字们记录下来，然后从服务器下载到本地。  

lua文件的加载：lua文件全部打包到同一个assetBundle，叫做lua.ab。由于lua的require函数无法直接加载ab包中的lua文本文件，所以在资源热更新完成后，将会在c#端加载并存储所有lua表：直接从lua.ab中加载出所有lua表，用<luafileName，luaTable>的形式缓存到一个字典里。以后在lua端想要使用require函数拿到luaTable的地方，就调用c#端的函数AssetManager.Require(luafileName)，取到缓存在字典中的luaTable。  

lua逻辑的进入：热更新由Asset/Scripts/FrameWork/UpdateVersion.cs执行，热更新完成后，会执行名为main的lua文件(Asset/Lua/FrameWork/main.txt)，由此进入lua逻辑。

lua与unity生命周期函数的交互：写了一个LuaBehaviour.cs脚本继承monoBehaviour，一个LuaBehaviour.cs绑定一个lua table。将start,update,fixedUpdate等方法套接到lua中执行。在lua端绑定的时候，代码应该像这样：  
    local characterAsset = AssetManager.LoadAsset('character.ab','character');--加载character.ab包中的character预制体  
    local characterGo = GameObject.Instantiate(characterAsset);--生成游戏物体  
    local Character = AssetManager.Require('character.txt');--从字典里取到名为character.txt的luaTable  
    local character = Character:new();--从Character 这张luaTable“实例化”出一个实例   
    characterGo:AddComponent(LuaBehaviour):BindLuaTable(character);--绑定到luaBehaviour以获取update等方法  

资源管理器AssetManager：想要从指定ab包中取出指定名字的资源，传入ab包名，资源名，资源管理器将加载指定ab包（若有依赖，依赖包也将加载）并取出指定资源，取出后，将直接assetBundle.UnLoad(false)把加载的ab包和依赖包都卸载掉。回收一部分内存。加载出来的asset将由assetManager添加到字典里管理，键为资源名，值为asset本身。下次再想加载这个资源将从字典里直接取出返回。切换关卡的时候需要清空这个字典。让Resources.UnLoadUnusedAssets()方法能够回收这些资源。  

对象池：写了一个gameObject的对象池，还未使用到。  

消息派发：使用监听者模式。消息中心维护了一个字典：Dictionary<string, List<LuaTable>> handlerDic。string就是事件名，list就是关心这个事件的所有table。没有使用c#的委托来实现监听者模式，因为那样做的话，在事件触发时，lua端的方法，拿不到self变量。  

有限状态机：在lua端写有限状态机框架，写了角色和敌人的有限状态机。  

场景管理器：异步加载场景的场景管理器  


项目的目录结构：  
![1](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/dicDescription.png)  
工具：  
![2](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/toolDescription.png)  
宏：本地开发调试时启用，不用每次调试都将ab包上传到服务器再运行，点击了Tools/a.->Tool/b，打包ab包完成， 即可本地调试运行：  
![3](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/defDescription.png)  

运行指引：  
下载项目后，还需要下载Slua：https://github.com/pangweiwei/slua  
导入完成后，打开Asset/Scenes/init场景。
不将资源上传到服务器，本地开发调试模式：启用宏：DEVELOP_MODE，然后使用工具打包:Tool/a. ->Tool/b. 即可运行游戏。（如果报错，那么运行Tool/e.清空缓存）  
将资源上传到服务器的模式：不启用宏，依次运行工具：Tool/a.b.c.d，然后将UpdatePackage文件夹下所有文件拖拽到服务器上(服务器地址修改位置在Assets/Scripts/FrameWork/UpdateVersion.cs/13行)，即可运行游戏。  

小游戏介绍：这是一个挂机小游戏，截图如下：  
游戏开始：角色会一直向前跑:  
![4](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running1.png)  
然后遇到敌人，角色和敌人就会开始自动攻击:  
![5](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running2.png)  
点击使用技能:  
![6](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running3.png)  
角色击败敌人就会继续前进，遇到红色道具是加血：  
![7](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running4.png)  
通过手指在屏幕上左右滑动，角色可以在三条跑道上切换：  
![8](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running5.png)  
遇到蓝色道具是提升攻击力:  
![9](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running6.png)  
活着到达跑到终点就是胜利，中途被打败就是失败:  
![10](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running7.png)  

