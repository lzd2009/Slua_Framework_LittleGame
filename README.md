ʹ��slua��һ���򵥵Ŀ�ܡ�  
��ܲ��ֶ���C#�˱�д��������  

��Դ�ȸ��£�����ÿ��assetBundle��md5ֵ->��¼��һ��version.txt�ļ���->��assetBundle�Ǻ�version.txt�ϴ������������ͻ�������ʱ�����ȿ���������version.txt�ļ������������version.txt�ļ����бȶԣ��ѱ���û�еĺ�md5ֵ��ͬ��ab���������Ǽ�¼������Ȼ��ӷ��������ص����ء�  

lua�ļ��ļ��أ�lua�ļ�ȫ�������ͬһ��assetBundle������lua.ab������lua��require�����޷�ֱ�Ӽ���ab���е�lua�ı��ļ�����������Դ�ȸ�����ɺ󣬽�����c#�˼��ز��洢����lua��ֱ�Ӵ�lua.ab�м��س�����lua����<luafileName��luaTable>����ʽ���浽һ���ֵ���Ժ���lua����Ҫʹ��require�����õ�luaTable�ĵط����͵���c#�˵ĺ���AssetManager.Require(luafileName)��ȡ���������ֵ��е�luaTable��  

lua�߼��Ľ��룺�ȸ�����Asset/Scripts/FrameWork/UpdateVersion.csִ�У��ȸ�����ɺ󣬻�ִ����Ϊmain��lua�ļ�(Asset/Lua/FrameWork/main.txt)���ɴ˽���lua�߼���

lua��unity�������ں����Ľ�����д��һ��LuaBehaviour.cs�ű��̳�monoBehaviour��һ��LuaBehaviour.cs��һ��lua table����start,update,fixedUpdate�ȷ����׽ӵ�lua��ִ�С���lua�˰󶨵�ʱ�򣬴���Ӧ����������  
    local characterAsset = AssetManager.LoadAsset('character.ab','character');--����character.ab���е�characterԤ����  
    local characterGo = GameObject.Instantiate(characterAsset);--������Ϸ����  
    local Character = AssetManager.Require('character.txt');--���ֵ���ȡ����Ϊcharacter.txt��luaTable  
    local character = Character:new();--��Character ����luaTable��ʵ��������һ��ʵ��   
    characterGo:AddComponent(LuaBehaviour):BindLuaTable(character);--�󶨵�luaBehaviour�Ի�ȡupdate�ȷ���  

��Դ������AssetManager����Ҫ��ָ��ab����ȡ��ָ�����ֵ���Դ������ab��������Դ������Դ������������ָ��ab��������������������Ҳ�����أ���ȡ��ָ����Դ��ȡ���󣬽�ֱ��assetBundle.UnLoad(false)�Ѽ��ص�ab������������ж�ص�������һ�����ڴ档���س�����asset����assetManager��ӵ��ֵ��������Ϊ��Դ����ֵΪasset�����´�������������Դ�����ֵ���ֱ��ȡ�����ء��л��ؿ���ʱ����Ҫ�������ֵ䡣��Resources.UnLoadUnusedAssets()�����ܹ�������Щ��Դ��  

����أ�д��һ��gameObject�Ķ���أ���δʹ�õ���  

��Ϣ�ɷ���ʹ�ü�����ģʽ����Ϣ����ά����һ���ֵ䣺Dictionary<string, List<LuaTable>> handlerDic��string�����¼�����list���ǹ�������¼�������table��û��ʹ��c#��ί����ʵ�ּ�����ģʽ����Ϊ�������Ļ������¼�����ʱ��lua�˵ķ������ò���self������  

����״̬������lua��д����״̬����ܣ�д�˽�ɫ�͵��˵�����״̬����  

�������������첽���س����ĳ���������  


��Ŀ��Ŀ¼�ṹ��  
![1](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/dicDescription.png)  
���ߣ�  
![2](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/toolDescription.png)  
�꣺���ؿ�������ʱ���ã�����ÿ�ε��Զ���ab���ϴ��������������У������Tools/a.->Tool/b�����ab����ɣ� ���ɱ��ص������У�  
![3](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/defDescription.png)  

����ָ����  
������Ŀ�󣬻���Ҫ����Slua��https://github.com/pangweiwei/slua  
������ɺ󣬴�Asset/Scenes/init������
������Դ�ϴ��������������ؿ�������ģʽ�����ú꣺DEVELOP_MODE��Ȼ��ʹ�ù��ߴ��:Tool/a. ->Tool/b. ����������Ϸ�������������ô����Tool/e.��ջ��棩  
����Դ�ϴ�����������ģʽ�������ú꣬�������й��ߣ�Tool/a.b.c.d��Ȼ��UpdatePackage�ļ����������ļ���ק����������(��������ַ�޸�λ����Assets/Scripts/FrameWork/UpdateVersion.cs/13��)������������Ϸ��  

С��Ϸ���ܣ�����һ���һ�С��Ϸ����ͼ���£�  
��Ϸ��ʼ����ɫ��һֱ��ǰ��:  
![4](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running1.png)  
Ȼ���������ˣ���ɫ�͵��˾ͻῪʼ�Զ�����:  
![5](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running2.png)  
���ʹ�ü���:  
![6](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running3.png)  
��ɫ���ܵ��˾ͻ����ǰ����������ɫ�����Ǽ�Ѫ��  
![7](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running4.png)  
ͨ����ָ����Ļ�����һ�������ɫ�����������ܵ����л���  
![8](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running5.png)  
������ɫ����������������:  
![9](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running6.png)  
���ŵ����ܵ��յ����ʤ������;����ܾ���ʧ��:  
![10](https://github.com/lzd2009/Slua_Framework_LittleGame/raw/master/showImage/running7.png)  

